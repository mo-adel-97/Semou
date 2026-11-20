<?php
declare(strict_types=1);

/* ============================================================
   Trainee portal. Login = national ID + the mobile number
   registered in the ERP. Separate cookie from the dashboard.
   Brute force: per-IP rate limit + per-account lockout.
   ============================================================ */

const PORTAL_COOKIE = 'alsmow_portal';
const PORTAL_HOURS = 8;
const PORTAL_MAX_FAILS = 5;
const PORTAL_LOCK_MINUTES = 15;

function portal_cookie(string $value, int $expires): void
{
    setcookie(PORTAL_COOKIE, $value, [
        'expires' => $expires,
        'path' => '/api/portal',
        'secure' => (bool) config('cookie_secure'),
        'httponly' => true,
        'samesite' => 'Strict',
    ]);
}

function portal_login(string $nationalId, string $phone): array
{
    rate_limit('portal', 12, 15 * 60);
    $nid = preg_replace('/\D+/', '', $nationalId) ?? '';
    $pk = phone_key($phone);
    $generic = new HttpError(401, 'رقم الهوية أو رقم الجوال غير صحيح');
    if (strlen($nid) < 8 || strlen($pk) < 9) throw $generic;

    $rows = DB::all('SELECT * FROM portal_students WHERE national_id = ? AND is_active = 1', [$nid]);
    if (!$rows) {
        hash_equals(str_repeat('0', 9), $pk); // keep timing similar
        throw $generic;
    }
    foreach ($rows as $s) {
        if ($s['locked_until'] && strtotime($s['locked_until']) > time()) {
            $mins = (int) ceil((strtotime($s['locked_until']) - time()) / 60);
            throw new HttpError(429, "تم إيقاف الدخول مؤقتًا بسبب محاولات خاطئة متكررة، حاول بعد $mins دقيقة");
        }
    }
    $match = null;
    foreach ($rows as $s) if (hash_equals($s['phone_key'], $pk)) $match = $s;

    if (!$match) {
        foreach ($rows as $s) {
            $fails = (int) $s['failed_attempts'] + 1;
            DB::run('UPDATE portal_students SET failed_attempts = ?, locked_until = ? WHERE id = ?', [
                $fails >= PORTAL_MAX_FAILS ? 0 : $fails,
                $fails >= PORTAL_MAX_FAILS ? date('Y-m-d H:i:s', time() + PORTAL_LOCK_MINUTES * 60) : null,
                $s['id'],
            ]);
        }
        throw $generic;
    }

    DB::run('UPDATE portal_students SET failed_attempts = 0, locked_until = NULL, last_login_at = NOW() WHERE id = ?', [$match['id']]);
    $exp = time() + PORTAL_HOURS * 3600;
    portal_cookie(jwt_sign(['sub' => (int) $match['id'], 'kind' => 'portal', 'exp' => $exp]), $exp);
    return portal_profile($match);
}

function portal_current(): array
{
    $claims = jwt_verify((string) ($_COOKIE[PORTAL_COOKIE] ?? ''));
    if (!$claims || ($claims['kind'] ?? '') !== 'portal') throw new HttpError(401, 'يرجى تسجيل الدخول');
    $s = DB::one('SELECT * FROM portal_students WHERE id = ? AND is_active = 1', [(int) $claims['sub']]);
    if (!$s) throw new HttpError(401, 'يرجى تسجيل الدخول');
    return $s;
}

function portal_profile(array $s): array
{
    $data = json_decode((string) $s['data'], true) ?: [];
    $nid = (string) $s['national_id'];
    return [
        'name' => $s['name'],
        'code' => $s['code'],
        'national_id' => str_repeat('•', max(0, strlen($nid) - 4)) . substr($nid, -4),
        'branch' => $data['branch'] ?? null,
        'programs' => $data['programs'] ?? [],
        'finance' => $data['finance'] ?? ['total' => 0, 'discount' => 0, 'paid' => 0, 'balance' => 0],
        'payments' => $data['payments'] ?? [],
        'synced_at' => $s['synced_at'],
    ];
}
