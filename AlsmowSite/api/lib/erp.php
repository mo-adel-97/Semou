<?php
declare(strict_types=1);

/* ============================================================
   Semou ERP integration (server-to-server, no cookies).

   The ERP backend (.NET, on the ERP server) is the only caller.
   Every request is signed with the shared secret `erp_secret`:

     X-Alsmow-Expires:   unix time, at most 5 minutes ahead
     X-Alsmow-Signature: base64url(HMAC_SHA256("<METHOD>|<path>|<query>|<expires>|<sha256(body) hex>", erp_secret))

   Endpoints
     GET  /api/erp/requests?after=<id>&limit=<n>   study requests (register form) newer than <id>
     POST /api/erp/requests/ack                    {"items":[{"id":12,"ref":"WSR-120"}]}
     POST /api/erp/students                        {"students":[StudentPayload...], "full": false}
     GET  /api/erp/status                          counts, for the ERP's health check

   StudentPayload (all strings unless noted):
     guid, national_id, phone, name, code, active(bool),
     programs: [{title, kind: "diploma"|"course", level, batch, branch, status, start_date, reg_code}],
     finance:  {total(num), discount(num), paid(num), balance(num)},
     payments: [{date, amount(num), ref, method, note}]
   ============================================================ */

const ERP_MAX_SKEW = 300;

function erp_verify(string $method, string $path): void
{
    $secret = (string) (config('erp_secret') ?? '');
    if (strlen($secret) < 32) throw new HttpError(503, 'ERP integration is not configured');

    $expires = (int) ($_SERVER['HTTP_X_ALSMOW_EXPIRES'] ?? 0);
    $sig = (string) ($_SERVER['HTTP_X_ALSMOW_SIGNATURE'] ?? '');
    $now = time();
    if ($expires < $now || $expires > $now + ERP_MAX_SKEW || $sig === '') throw new HttpError(403, 'invalid signature');

    $query = $_SERVER['QUERY_STRING'] ?? '';
    $body = file_get_contents('php://input') ?: '';
    $base = implode('|', [$method, $path, $query, (string) $expires, hash('sha256', $body)]);
    $expected = rtrim(strtr(base64_encode(hash_hmac('sha256', $base, $secret, true)), '+/', '-_'), '=');
    if (!hash_equals($expected, $sig)) throw new HttpError(403, 'invalid signature');
}

function erp_log(string $direction, int $items, ?string $note = null): void
{
    DB::insert('erp_sync_log', ['direction' => $direction, 'items' => $items, 'note' => $note]);
    DB::run('DELETE FROM erp_sync_log WHERE created_at < (NOW() - INTERVAL 30 DAY)');
}

function erp_requests(int $after, int $limit): array
{
    $limit = max(1, min(500, $limit));
    $rows = DB::all(
        "SELECT id, name, phone, email, national_id, city, program, study_mode, body, created_at
         FROM messages WHERE kind = 'register' AND id > ? ORDER BY id LIMIT $limit",
        [$after]
    );
    return [
        'items' => array_map(fn($r) => ['id' => (int) $r['id']] + $r, $rows),
        'last_id' => $rows ? (int) end($rows)['id'] : $after,
    ];
}

function erp_ack(array $items): array
{
    $n = 0;
    foreach ($items as $it) {
        $id = (int) ($it['id'] ?? 0);
        if ($id <= 0) continue;
        $n += DB::run('UPDATE messages SET erp_synced_at = NOW(), erp_ref = ? WHERE id = ? AND kind = "register"', [
            mb_substr((string) ($it['ref'] ?? ''), 0, 60) ?: null, $id,
        ])->rowCount();
    }
    erp_log('requests', $n, 'ack');
    return ['acked' => $n];
}

function phone_key(string $phone): string
{
    $d = preg_replace('/\D+/', '', $phone) ?? '';
    return substr($d, -9); // 05XXXXXXXX / 9665XXXXXXXX / 5XXXXXXXX all end in the same 9 digits
}

function erp_upsert_students(array $students, bool $full): array
{
    $num = fn($v) => is_numeric($v) ? round((float) $v, 2) : 0.0;
    $str = fn($v, int $max = 200) => $v === null ? null : mb_substr(trim((string) $v), 0, $max);
    $seen = [];
    $count = 0;
    $pdo = DB::pdo();
    $pdo->beginTransaction();
    try {
        foreach ($students as $s) {
            $guid = strtolower((string) ($s['guid'] ?? ''));
            $nid = preg_replace('/\D+/', '', (string) ($s['national_id'] ?? '')) ?? '';
            $pk = phone_key((string) ($s['phone'] ?? ''));
            if (!preg_match('/^[0-9a-f-]{36}$/', $guid) || strlen($nid) < 8 || strlen($pk) < 9) continue;
            $data = [
                'branch' => $str($s['branch'] ?? null),
                'programs' => array_values(array_map(fn($p) => [
                    'title' => $str($p['title'] ?? ''),
                    'kind' => ($p['kind'] ?? '') === 'course' ? 'course' : 'diploma',
                    'level' => $str($p['level'] ?? null, 80),
                    'batch' => $str($p['batch'] ?? null, 80),
                    'branch' => $str($p['branch'] ?? null, 120),
                    'status' => $str($p['status'] ?? null, 80),
                    'start_date' => $str($p['start_date'] ?? null, 20),
                    'reg_code' => $str($p['reg_code'] ?? null, 40),
                ], array_slice(is_array($s['programs'] ?? null) ? $s['programs'] : [], 0, 30))),
                'finance' => [
                    'total' => $num($s['finance']['total'] ?? 0),
                    'discount' => $num($s['finance']['discount'] ?? 0),
                    'paid' => $num($s['finance']['paid'] ?? 0),
                    'balance' => $num($s['finance']['balance'] ?? 0),
                ],
                'payments' => array_values(array_map(fn($p) => [
                    'date' => $str($p['date'] ?? null, 20),
                    'amount' => $num($p['amount'] ?? 0),
                    'ref' => $str($p['ref'] ?? null, 60),
                    'method' => $str($p['method'] ?? null, 60),
                    'note' => $str($p['note'] ?? null, 200),
                ], array_slice(is_array($s['payments'] ?? null) ? $s['payments'] : [], 0, 200))),
            ];
            DB::run(
                'INSERT INTO portal_students (erp_guid, national_id, phone_key, name, code, is_active, data, synced_at)
                 VALUES (?, ?, ?, ?, ?, ?, ?, NOW())
                 ON DUPLICATE KEY UPDATE national_id = VALUES(national_id), phone_key = VALUES(phone_key), name = VALUES(name),
                   code = VALUES(code), is_active = VALUES(is_active), data = VALUES(data), synced_at = NOW()',
                [$guid, $nid, $pk, $str($s['name'] ?? '') ?: '—', $str($s['code'] ?? null, 40), !empty($s['active']) || !array_key_exists('active', $s) ? 1 : 0,
                 json_encode($data, JSON_UNESCAPED_UNICODE)]
            );
            $seen[] = $guid;
            $count++;
        }
        $removed = 0;
        if ($full && $seen) {
            // A full sync lists every student: anyone missing no longer has portal access.
            $in = implode(',', array_fill(0, count($seen), '?'));
            $removed = DB::run("UPDATE portal_students SET is_active = 0 WHERE erp_guid NOT IN ($in)", $seen)->rowCount();
        }
        $pdo->commit();
    } catch (Throwable $e) {
        $pdo->rollBack();
        throw $e;
    }
    erp_log('students', $count, $full ? "full, deactivated $removed" : 'delta');
    return ['upserted' => $count, 'deactivated' => $removed];
}

/** The ERP sends the full list of current student guids; anyone else loses portal access. */
function erp_prune_students(array $guids): array
{
    $keep = array_values(array_filter(array_map(fn($g) => strtolower((string) $g), $guids), fn($g) => preg_match('/^[0-9a-f-]{36}$/', $g)));
    if (!$keep) throw new HttpError(422, 'empty guid list'); // never wipe everyone by mistake
    DB::run('CREATE TEMPORARY TABLE keep_guids (g CHAR(36) PRIMARY KEY)');
    foreach (array_chunk($keep, 500) as $chunk) {
        DB::run('INSERT IGNORE INTO keep_guids (g) VALUES ' . implode(',', array_fill(0, count($chunk), '(?)')), $chunk);
    }
    $n = DB::run('UPDATE portal_students SET is_active = 0 WHERE is_active = 1 AND erp_guid NOT IN (SELECT g FROM keep_guids)')->rowCount();
    erp_log('students', count($keep), "prune, deactivated $n");
    return ['kept' => count($keep), 'deactivated' => $n];
}

function erp_status(): array
{
    return [
        'students' => (int) DB::value('SELECT COUNT(*) FROM portal_students WHERE is_active = 1'),
        'pending_requests' => (int) DB::value("SELECT COUNT(*) FROM messages WHERE kind = 'register' AND erp_synced_at IS NULL"),
        'last_sync' => DB::one('SELECT direction, items, note, created_at FROM erp_sync_log ORDER BY id DESC LIMIT 1'),
    ];
}
