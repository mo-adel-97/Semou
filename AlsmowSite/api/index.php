<?php
declare(strict_types=1);

/* ============================================================
   Alsmow site API. Front controller for every /api/* request.
   ============================================================ */

require __DIR__ . '/lib/core.php';
require __DIR__ . '/lib/auth.php';
require __DIR__ . '/lib/content.php';
require __DIR__ . '/lib/resources.php';
require __DIR__ . '/lib/settings.php';
require __DIR__ . '/lib/search.php';

date_default_timezone_set('Asia/Riyadh');
mb_internal_encoding('UTF-8');
ini_set('display_errors', '0');

header('X-Content-Type-Options: nosniff');
header('Referrer-Policy: strict-origin-when-cross-origin');
header('Cache-Control: no-store');

set_exception_handler(function (Throwable $e): void {
    if ($e instanceof HttpError) {
        $payload = ['message' => $e->getMessage()];
        if ($e->errors) $payload['errors'] = $e->errors;
        json_out($payload, $e->status);
    }
    error_log('[alsmow-api] ' . $e);
    json_out(['message' => 'حدث خطأ غير متوقع في الخادم، حاول مرة أخرى'], 500);
});

$method = $_SERVER['REQUEST_METHOD'] ?? 'GET';
$path = parse_url($_SERVER['REQUEST_URI'] ?? '/', PHP_URL_PATH) ?: '/';
$path = '/' . trim(preg_replace('#^.*?/api(?=/|$)#', '', $path), '/');

// Local dev only (php -S + remote DB through a tunnel, one request at a time): keep public GET answers for a
// few minutes so pages open instantly; any write (dashboard save, ERP sync...) clears them. Hosting is unchanged.
if (PHP_SAPI === 'cli-server') {
    $devCache = sys_get_temp_dir() . DIRECTORY_SEPARATOR . 'alsmow-dev-cache';
    if (!is_dir($devCache)) @mkdir($devCache, 0777, true);
    // (visitor writes like page tracking and chat don't change content: they keep the cache)
    $isRead = $method === 'GET' || $method === 'HEAD';
    if (!$isRead) {
        // only real content changes clear the cache; a chat question or a page-view hit never does
        if (!in_array($path, ['/public/track', '/public/chat'], true)) {
            foreach (glob($devCache . DIRECTORY_SEPARATOR . '*.json') ?: [] as $f) @unlink($f);
        }
    } elseif (str_starts_with($path, '/public/')) { // reads only: never a POST (each chat question is different)
        $devFile = $devCache . DIRECTORY_SEPARATOR . md5($_SERVER['REQUEST_URI'] ?? $path) . '.json';
        if (is_file($devFile) && time() - filemtime($devFile) < 300) {
            header('Content-Type: application/json; charset=utf-8');
            header('X-Dev-Cache: hit');
            readfile($devFile);
            exit;
        }
        ob_start(function (string $buf) use ($devFile) {
            if (http_response_code() === 200 && $buf !== '') @file_put_contents($devFile, $buf);
            return $buf;
        });
    }
}

// Browser writes need the CSRF header; ERP calls are HMAC-signed instead and carry no cookies.
if ($method !== 'GET' && $method !== 'HEAD' && !str_starts_with($path, '/erp/')) {
    require_csrf_header();
}
require __DIR__ . '/lib/erp.php';
require __DIR__ . '/lib/portal.php';

$r = new Router();

/* ================= Public ================= */

$publicCache = ['Cache-Control' => 'public, max-age=60'];

$r->get('/health', fn() => ['ok' => true, 'db' => (bool) DB::value('SELECT 1')]);

$r->get('/public/home', function () use ($publicCache) {
    $programs = resource('programs');
    $rows = DB::all(
        'SELECT * FROM programs WHERE is_published = 1 AND is_featured = 1 ORDER BY sort ASC, id DESC LIMIT 24'
    );
    $posts = resource('posts');
    json_out([
        'settings' => public_settings(),
        'programs' => array_map(fn($x) => shape_row($programs, $x), $rows),
        'posts' => array_map(fn($x) => shape_row($posts, $x), DB::all(
            'SELECT id, title, slug, excerpt, cover, category, published_at FROM posts
             WHERE is_published = 1 ORDER BY published_at DESC, id DESC LIMIT 3'
        )),
        'accreditations' => public_list('accreditations'),
        'partners' => public_list('partners'),
        'agreements' => public_list('agreements', 6),
        'testimonials' => public_list('testimonials', 12),
        'counts' => program_counts(),
    ], 200, $publicCache);
});

$r->get('/public/site', function () use ($publicCache) {
    json_out([
        'settings' => public_settings(),
        'pages' => DB::all('SELECT title, slug FROM pages WHERE is_published = 1 AND in_menu = 1 ORDER BY sort ASC, id ASC'),
        'counts' => program_counts(),
    ], 200, $publicCache);
});

$r->get('/public/programs', function () use ($publicCache) {
    $res = resource('programs');
    $type = $_GET['type'] ?? '';
    $where = 'is_published = 1';
    $params = [];
    if ($type !== '' && isset(PROGRAM_TYPES[$type])) { $where .= ' AND type = ?'; $params[] = $type; }
    $rows = DB::all("SELECT * FROM programs WHERE $where ORDER BY sort ASC, id DESC", $params);
    foreach ($rows as &$row) $row['body'] = null; // listing doesn't need the full body
    json_out(array_map(fn($x) => shape_row($res, $x), $rows), 200, $publicCache);
});

$r->get('/public/programs/{slug}', function ($p) use ($publicCache) {
    $res = resource('programs');
    $row = DB::one('SELECT * FROM programs WHERE slug = ? AND is_published = 1', [$p['slug']]);
    if (!$row) throw new HttpError(404, 'البرنامج غير موجود');
    $related = DB::all(
        'SELECT * FROM programs WHERE is_published = 1 AND type = ? AND id <> ? ORDER BY sort ASC, id DESC LIMIT 3',
        [$row['type'], $row['id']]
    );
    json_out([
        'program' => shape_row($res, $row),
        'related' => array_map(fn($x) => shape_row($res, ['body' => null] + $x), $related),
    ], 200, $publicCache);
});

$r->get('/public/posts', function () use ($publicCache) {
    $res = resource('posts');
    $page = max(1, (int) ($_GET['page'] ?? 1));
    $per = 9;
    $total = (int) DB::value('SELECT COUNT(*) FROM posts WHERE is_published = 1');
    $rows = DB::all(
        'SELECT id, title, slug, excerpt, cover, category, published_at FROM posts WHERE is_published = 1
         ORDER BY published_at DESC, id DESC LIMIT ' . $per . ' OFFSET ' . (($page - 1) * $per)
    );
    json_out([
        'items' => array_map(fn($x) => shape_row($res, $x), $rows),
        'page' => $page,
        'pages' => max(1, (int) ceil($total / $per)),
        'total' => $total,
    ], 200, $publicCache);
});

$r->get('/public/posts/{slug}', function ($p) use ($publicCache) {
    $res = resource('posts');
    $row = DB::one('SELECT * FROM posts WHERE slug = ? AND is_published = 1', [$p['slug']]);
    if (!$row) throw new HttpError(404, 'المقال غير موجود');
    $more = DB::all(
        'SELECT id, title, slug, excerpt, cover, category, published_at FROM posts
         WHERE is_published = 1 AND id <> ? ORDER BY published_at DESC, id DESC LIMIT 3',
        [$row['id']]
    );
    json_out([
        'post' => shape_row($res, $row),
        'more' => array_map(fn($x) => shape_row($res, $x), $more),
    ], 200, $publicCache);
});

$r->get('/public/pages/{slug}', function ($p) use ($publicCache) {
    $row = DB::one('SELECT title, slug, body, updated_at FROM pages WHERE slug = ? AND is_published = 1', [$p['slug']]);
    if (!$row) throw new HttpError(404, 'الصفحة غير موجودة');
    json_out($row, 200, $publicCache);
});

$r->post('/public/messages', function () {
    $in = body();
    // Honeypot: real people never fill the hidden "website" field.
    if (!empty($in['website'])) return ['ok' => true];
    rate_limit('message', 5, 10 * 60);

    $errors = [];
    $name = plain_text($in['name'] ?? null, 120);
    $phone = plain_text($in['phone'] ?? null, 30);
    $email = plain_text($in['email'] ?? null, 190);
    $kind = ($in['kind'] ?? '') === 'register' ? 'register' : 'contact';
    $text = plain_text($in['body'] ?? null, 3000);
    if (!$name || mb_strlen($name) < 3) $errors['name'] = 'يرجى كتابة الاسم';
    if (!$phone || !preg_match('/^[0-9+\s-]{8,20}$/', $phone)) $errors['phone'] = 'رقم الجوال غير صحيح';
    if ($email && !filter_var($email, FILTER_VALIDATE_EMAIL)) $errors['email'] = 'البريد الإلكتروني غير صحيح';
    if ($kind === 'contact' && !$text) $errors['body'] = 'يرجى كتابة رسالتك';
    $mode = in_array($in['study_mode'] ?? '', ['onsite', 'online'], true) ? $in['study_mode'] : null;
    if ($kind === 'register' && !$mode) $errors['study_mode'] = 'اختر طريقة الدراسة';
    $nid = preg_replace('/\D+/', '', (string) ($in['national_id'] ?? '')) ?: null;
    if ($nid !== null && !preg_match('/^[12]\d{9}$/', $nid)) $errors['national_id'] = 'رقم الهوية أو الإقامة يتكون من 10 أرقام';
    if ($errors) throw new HttpError(422, 'يرجى مراجعة الحقول المظللة', $errors);

    $id = DB::insert('messages', [
        'kind' => $kind,
        'name' => $name,
        'phone' => $phone,
        'email' => $email,
        'national_id' => $kind === 'register' ? $nid : null,
        'city' => $kind === 'register' ? plain_text($in['city'] ?? null, 80) : null,
        'study_mode' => $kind === 'register' ? $mode : null,
        'program' => plain_text($in['program'] ?? null, 220),
        'subject' => plain_text($in['subject'] ?? null, 200),
        'body' => $text,
        'ip' => client_ip(),
    ]);
    notify_new_message($id);
    return ['ok' => true];
});

/* ================= Trainee portal ================= */

$r->post('/portal/login', function () {
    $in = body();
    return ['student' => portal_login((string) ($in['national_id'] ?? ''), (string) ($in['phone'] ?? ''))];
});
$r->get('/portal/me', fn() => ['student' => portal_profile(portal_current())]);
$r->post('/portal/logout', function () {
    portal_cookie('', time() - 3600);
    return ['ok' => true];
});

/* ================= ERP integration (HMAC-signed) ================= */

$r->get('/erp/status', function () {
    erp_verify('GET', '/api/erp/status');
    return erp_status();
});
$r->get('/erp/requests', function () {
    erp_verify('GET', '/api/erp/requests');
    return erp_requests((int) ($_GET['after'] ?? 0), (int) ($_GET['limit'] ?? 200));
});
$r->post('/erp/requests/ack', function () {
    erp_verify('POST', '/api/erp/requests/ack');
    return erp_ack(is_array(body()['items'] ?? null) ? body()['items'] : []);
});
$r->post('/erp/students', function () {
    erp_verify('POST', '/api/erp/students');
    $in = body();
    return erp_upsert_students(is_array($in['students'] ?? null) ? $in['students'] : [], !empty($in['full']));
});

$r->post('/erp/students/prune', function () {
    erp_verify('POST', '/api/erp/students/prune');
    return erp_prune_students(is_array(body()['guids'] ?? null) ? body()['guids'] : []);
});

$r->get('/admin/erp-status', function () {
    require_user();
    return erp_status() + ['log' => DB::all('SELECT direction, items, note, created_at FROM erp_sync_log ORDER BY id DESC LIMIT 15')];
});

/* Analytics: page views / time on page / events from the browser (lib/analytics.php) */
$r->post('/public/track', function () {
    require_once __DIR__ . '/lib/analytics.php';
    try { an_track(body()); } catch (Throwable $e) { error_log('[alsmow-analytics] ' . $e->getMessage()); }
    http_response_code(204);
    exit;
});

$r->get('/admin/analytics', function () {
    require_user();
    require_once __DIR__ . '/lib/analytics.php';
    return an_report((string) ($_GET['from'] ?? ''), (string) ($_GET['to'] ?? ''));
});

/* «تحدث إلى السمو»: answered by the institute's own assistant (lib/assistant.php), no external AI */
$r->post('/public/chat', function () {
    require_once __DIR__ . '/lib/assistant.php';
    if (!as_settings()['enabled']) throw new HttpError(503, 'المساعد متوقف حاليًا');
    rate_limit('chat', 60, 15 * 60);
    $in = body();
    $history = [];
    foreach (array_slice(is_array($in['history'] ?? null) ? $in['history'] : [], -8) as $m) {
        if (!is_array($m)) continue;
        $history[] = ['role' => ($m['role'] ?? '') === 'assistant' ? 'assistant' : 'user', 'content' => mb_substr((string) ($m['content'] ?? ''), 0, 1500)];
    }
    $res = as_answer((string) ($in['q'] ?? ''), $history);
    // questions it could not answer are kept for the dashboard (المساعد الذكي › أسئلة بلا إجابة)
    if (!$res['confident']) as_log_unanswered((string) ($in['q'] ?? ''));
    return $res;
});

$r->get('/admin/assistant/unanswered', function () {
    require_user();
    require_once __DIR__ . '/lib/assistant.php';
    return as_unanswered();
});

$r->delete('/admin/assistant/unanswered', function () {
    require_user();
    require_once __DIR__ . '/lib/assistant.php';
    as_unanswered_remove(isset($_GET['k']) ? (string) $_GET['k'] : null);
    return ['ok' => true];
});

/* Dashboard: try the assistant with the knowledge base being edited (not saved yet) */
$r->post('/admin/assistant/test', function () {
    require_user();
    require_once __DIR__ . '/lib/assistant.php';
    $in = body();
    $kb = [];
    foreach (is_array($in['kb'] ?? null) ? $in['kb'] : [] as $e) {
        if (!is_array($e)) continue;
        $kb[] = ['q' => (string) ($e['q'] ?? ''), 'a' => (string) ($e['a'] ?? ''), 'link' => (string) ($e['link'] ?? ''), 'variants' => is_array($e['variants'] ?? null) ? array_map('strval', $e['variants']) : []];
    }
    return as_answer((string) ($in['q'] ?? ''), is_array($in['history'] ?? null) ? $in['history'] : [], $kb);
});

/* ================= Auth ================= */

$r->post('/auth/login', function () {
    $in = body();
    return ['user' => login((string) ($in['email'] ?? ''), (string) ($in['password'] ?? ''))];
});

$r->post('/auth/logout', function () {
    logout();
    return ['ok' => true];
});

$r->get('/auth/me', function () {
    $u = current_user();
    if (!$u) throw new HttpError(401, 'غير مسجل الدخول');
    return ['user' => public_user($u)];
});

$r->post('/auth/password', function () {
    $u = require_user();
    $in = body();
    if (!password_verify((string) ($in['current'] ?? ''), $u['password_hash'])) {
        throw new HttpError(422, 'تحقق من البيانات', ['current' => 'كلمة المرور الحالية غير صحيحة']);
    }
    $new = (string) ($in['password'] ?? '');
    validate_password($new);
    $ver = (int) $u['token_version'] + 1;
    DB::update('users', (int) $u['id'], ['password_hash' => password_hash($new, PASSWORD_DEFAULT), 'token_version' => $ver]);
    // re-issue this session; every other session is now revoked
    $exp = time() + AUTH_HOURS * 3600;
    set_auth_cookie(jwt_sign(['sub' => (int) $u['id'], 'ver' => $ver, 'exp' => $exp]), $exp);
    log_activity($u, 'password', 'users', (int) $u['id'], $u['name']);
    return ['ok' => true];
});

/* ================= Admin ================= */

$r->get('/admin/schema', function () {
    require_user();
    return ['resources' => admin_schema(), 'settings' => settings_admin_schema()];
});

$r->get('/admin/dashboard', function () {
    $u = require_user();
    $counts = [];
    foreach (array_keys(resources()) as $name) {
        $counts[$name] = (int) DB::value("SELECT COUNT(*) FROM `$name`");
    }
    $daily = DB::all(
        "SELECT DATE(created_at) d, COUNT(*) c FROM messages
         WHERE created_at >= (CURDATE() - INTERVAL 29 DAY) GROUP BY DATE(created_at) ORDER BY d"
    );
    return [
        'user' => public_user($u),
        'counts' => $counts,
        'messages' => [
            'unread' => (int) DB::value('SELECT COUNT(*) FROM messages WHERE is_read = 0'),
            'total' => (int) DB::value('SELECT COUNT(*) FROM messages'),
            'registrations' => (int) DB::value("SELECT COUNT(*) FROM messages WHERE kind = 'register'"),
            'daily' => array_map(fn($x) => ['date' => $x['d'], 'count' => (int) $x['c']], $daily),
            'latest' => DB::all('SELECT id, kind, name, phone, program, subject, is_read, created_at FROM messages ORDER BY id DESC LIMIT 6'),
        ],
        'programsByType' => program_counts(),
        'activity' => DB::all(
            'SELECT a.action, a.resource, a.label, a.created_at, u.name AS user
             FROM activity_log a LEFT JOIN users u ON u.id = a.user_id ORDER BY a.id DESC LIMIT 8'
        ),
    ];
});

/* ---- Settings ---- */
$r->get('/admin/settings', function () {
    require_user();
    return all_settings();
});

$r->put('/admin/settings/{group}', function ($p) {
    $u = require_user();
    $saved = save_settings_group($p['group'], body());
    log_activity($u, 'update', 'settings', null, settings_schema()[$p['group']]['label']);
    return $saved;
});

/* ---- Media ---- */
$r->post('/admin/upload', function () {
    $u = require_user();
    rate_limit('upload', 120, 3600);
    return store_upload($_FILES['file'] ?? [], $u);
});

$r->get('/admin/media', function () {
    require_user();
    $page = max(1, (int) ($_GET['page'] ?? 1));
    $per = 40;
    return [
        'items' => DB::all('SELECT id, url, original_name, width, height, size, created_at FROM media ORDER BY id DESC LIMIT ' . $per . ' OFFSET ' . (($page - 1) * $per)),
        'pages' => max(1, (int) ceil(((int) DB::value('SELECT COUNT(*) FROM media')) / $per)),
        'page' => $page,
    ];
});

$r->delete('/admin/media/{id}', function ($p) {
    $u = require_user('admin');
    $m = DB::one('SELECT * FROM media WHERE id = ?', [(int) $p['id']]);
    if (!$m) throw new HttpError(404, 'الملف غير موجود');
    delete_media_file($m['url']);
    DB::run('DELETE FROM media WHERE id = ?', [(int) $m['id']]);
    log_activity($u, 'delete', 'media', (int) $m['id'], $m['original_name']);
    return ['ok' => true];
});

/* ---- Messages ---- */
$r->get('/admin/messages', function () {
    require_user();
    $where = ['1=1'];
    $params = [];
    if (in_array($_GET['kind'] ?? '', ['contact', 'register'], true)) { $where[] = 'kind = ?'; $params[] = $_GET['kind']; }
    if (($_GET['status'] ?? '') === 'unread') $where[] = 'is_read = 0';
    if (($q = trim((string) ($_GET['q'] ?? ''))) !== '') {
        $where[] = '(name LIKE ? OR phone LIKE ? OR email LIKE ? OR program LIKE ?)';
        array_push($params, "%$q%", "%$q%", "%$q%", "%$q%");
    }
    $page = max(1, (int) ($_GET['page'] ?? 1));
    $per = 25;
    $w = implode(' AND ', $where);
    $total = (int) DB::value("SELECT COUNT(*) FROM messages WHERE $w", $params);
    return [
        'items' => array_map(function ($m) {
            $m['is_read'] = (bool) $m['is_read'];
            unset($m['ip']);
            return $m;
        }, DB::all("SELECT * FROM messages WHERE $w ORDER BY id DESC LIMIT $per OFFSET " . (($page - 1) * $per), $params)),
        'total' => $total,
        'page' => $page,
        'pages' => max(1, (int) ceil($total / $per)),
    ];
});

$r->put('/admin/messages/{id}', function ($p) {
    require_user();
    DB::run('UPDATE messages SET is_read = ? WHERE id = ?', [!empty(body()['is_read']) ? 1 : 0, (int) $p['id']]);
    return ['ok' => true];
});

$r->delete('/admin/messages/{id}', function ($p) {
    $u = require_user('admin');
    DB::run('DELETE FROM messages WHERE id = ?', [(int) $p['id']]);
    log_activity($u, 'delete', 'messages', (int) $p['id'], null);
    return ['ok' => true];
});

$r->get('/admin/messages-export', function () {
    require_user();
    $rows = DB::all('SELECT id, kind, name, phone, email, program, subject, body, is_read, created_at FROM messages ORDER BY id DESC');
    header('Content-Type: text/csv; charset=utf-8');
    header('Content-Disposition: attachment; filename="messages-' . date('Y-m-d') . '.csv"');
    $out = fopen('php://output', 'w');
    fwrite($out, "\xEF\xBB\xBF"); // BOM so Excel shows Arabic correctly
    fputcsv($out, ['#', 'النوع', 'الاسم', 'الجوال', 'البريد', 'البرنامج', 'الموضوع', 'الرسالة', 'مقروءة', 'التاريخ']);
    foreach ($rows as $m) {
        $m['kind'] = $m['kind'] === 'register' ? 'طلب تسجيل' : 'تواصل';
        $m['is_read'] = $m['is_read'] ? 'نعم' : 'لا';
        // neutralise spreadsheet formulas (CSV injection)
        fputcsv($out, array_map(fn($v) => is_string($v) && preg_match('/^[=+\-@\t\r]/', $v) ? "'$v" : $v, array_values($m)));
    }
    fclose($out);
    exit;
});

/* ---- Users (admin only) ---- */
$r->get('/admin/users', function () {
    require_user('admin');
    return array_map(fn($u) => public_user($u) + [
        'is_active' => (bool) $u['is_active'],
        'last_login_at' => $u['last_login_at'],
        'created_at' => $u['created_at'],
    ], DB::all('SELECT * FROM users ORDER BY id'));
});

$r->post('/admin/users', function () {
    $admin = require_user('admin');
    $in = body();
    $data = validate_user_input($in, null);
    validate_password((string) ($in['password'] ?? ''));
    $data['password_hash'] = password_hash((string) $in['password'], PASSWORD_DEFAULT);
    $id = DB::insert('users', $data);
    log_activity($admin, 'create', 'users', $id, $data['name']);
    return ['id' => $id];
});

$r->put('/admin/users/{id}', function ($p) {
    $admin = require_user('admin');
    $id = (int) $p['id'];
    $existing = DB::one('SELECT * FROM users WHERE id = ?', [$id]) ?? throw new HttpError(404, 'المستخدم غير موجود');
    $in = body();
    $data = validate_user_input($in, $existing);
    if ($id === (int) $admin['id'] && ($data['role'] !== 'admin' || !$data['is_active'])) {
        throw new HttpError(422, 'لا يمكنك إيقاف حسابك أو إزالة صلاحية المدير عن نفسك');
    }
    if (($in['password'] ?? '') !== '') {
        validate_password((string) $in['password']);
        $data['password_hash'] = password_hash((string) $in['password'], PASSWORD_DEFAULT);
        $data['token_version'] = (int) $existing['token_version'] + 1;
    } elseif (!$data['is_active'] || $data['role'] !== $existing['role']) {
        $data['token_version'] = (int) $existing['token_version'] + 1; // force re-login
    }
    DB::update('users', $id, $data);
    log_activity($admin, 'update', 'users', $id, $data['name']);
    return ['ok' => true];
});

$r->delete('/admin/users/{id}', function ($p) {
    $admin = require_user('admin');
    $id = (int) $p['id'];
    if ($id === (int) $admin['id']) throw new HttpError(422, 'لا يمكنك حذف حسابك');
    DB::run('DELETE FROM users WHERE id = ?', [$id]);
    log_activity($admin, 'delete', 'users', $id, null);
    return ['ok' => true];
});

/* ---- Dashboard search (where is this text edited?) ---- */
$r->get('/admin/search', function () {
    require_user();
    return admin_search((string) ($_GET['q'] ?? ''));
});

/* ---- Generic content CRUD ---- */
$r->get('/admin/content/{res}', function ($p) {
    require_user();
    $res = resource($p['res']);
    $where = ['1=1'];
    $params = [];
    if (($q = trim((string) ($_GET['q'] ?? ''))) !== '') {
        $where[] = '(' . implode(' OR ', array_map(fn($c) => "`$c` LIKE ?", $res['search'])) . ')';
        foreach ($res['search'] as $_) $params[] = "%$q%";
    }
    foreach ($res['filters'] ?? [] as $f) {
        $val = $_GET[$f] ?? '';
        if ($val !== '' && isset($res['fields'][$f]['options'][$val])) { $where[] = "`$f` = ?"; $params[] = $val; }
    }
    $rows = DB::all("SELECT * FROM `{$res['table']}` WHERE " . implode(' AND ', $where) . " ORDER BY {$res['order']}", $params);
    return array_map(function ($row) use ($res) {
        $shaped = shape_row($res, $row);
        foreach ($res['fields'] as $k => $f) if ($f['type'] === 'html') unset($shaped[$k]); // keep the list light
        return $shaped;
    }, $rows);
});

$r->get('/admin/content/{res}/{id}', function ($p) {
    require_user();
    $res = resource($p['res']);
    $row = DB::one("SELECT * FROM `{$res['table']}` WHERE id = ?", [(int) $p['id']]);
    if (!$row) throw new HttpError(404, 'العنصر غير موجود');
    return shape_row($res, $row);
});

$r->post('/admin/content/{res}', function ($p) {
    $u = require_user();
    $res = resource($p['res']);
    $data = validate_record($res, body(), null);
    $id = DB::insert($res['table'], $data);
    log_activity($u, 'create', $res['name'], $id, (string) ($data[$res['title']] ?? ''));
    return shape_row($res, DB::one("SELECT * FROM `{$res['table']}` WHERE id = ?", [$id]));
});

$r->put('/admin/content/{res}/{id}', function ($p) {
    $u = require_user();
    $res = resource($p['res']);
    $id = (int) $p['id'];
    $existing = DB::one("SELECT * FROM `{$res['table']}` WHERE id = ?", [$id]);
    if (!$existing) throw new HttpError(404, 'العنصر غير موجود');
    $data = validate_record($res, body(), $existing);
    DB::update($res['table'], $id, $data);
    log_activity($u, 'update', $res['name'], $id, (string) ($data[$res['title']] ?? $existing[$res['title']]));
    return shape_row($res, DB::one("SELECT * FROM `{$res['table']}` WHERE id = ?", [$id]));
});

$r->delete('/admin/content/{res}/{id}', function ($p) {
    $u = require_user();
    $res = resource($p['res']);
    $id = (int) $p['id'];
    $existing = DB::one("SELECT * FROM `{$res['table']}` WHERE id = ?", [$id]);
    if (!$existing) throw new HttpError(404, 'العنصر غير موجود');
    DB::run("DELETE FROM `{$res['table']}` WHERE id = ?", [$id]);
    log_activity($u, 'delete', $res['name'], $id, (string) $existing[$res['title']]);
    return ['ok' => true];
});

$r->post('/admin/content/{res}/reorder', function ($p) {
    require_user();
    $res = resource($p['res']);
    if (!isset($res['fields']['sort'])) throw new HttpError(422, 'هذا المحتوى لا يدعم الترتيب');
    $ids = array_values(array_filter(array_map('intval', body()['ids'] ?? [])));
    $pdo = DB::pdo();
    $pdo->beginTransaction();
    foreach ($ids as $i => $id) DB::run("UPDATE `{$res['table']}` SET sort = ? WHERE id = ?", [$i, $id]);
    $pdo->commit();
    return ['ok' => true];
});

$r->dispatch($method, $path);

/* ================= Helpers ================= */

function public_list(string $name, int $limit = 60): array
{
    $res = resource($name);
    $rows = DB::all("SELECT * FROM `{$res['table']}` WHERE is_published = 1 ORDER BY {$res['order']} LIMIT $limit");
    return array_map(fn($x) => shape_row($res, $x), $rows);
}

function program_counts(): array
{
    $out = array_fill_keys(array_keys(PROGRAM_TYPES), 0);
    foreach (DB::all('SELECT type, COUNT(*) c FROM programs WHERE is_published = 1 GROUP BY type') as $r) {
        $out[$r['type']] = (int) $r['c'];
    }
    return $out;
}

function validate_user_input(array $in, ?array $existing): array
{
    $errors = [];
    $name = plain_text($in['name'] ?? null, 120);
    $email = mb_strtolower(trim((string) ($in['email'] ?? '')));
    $role = in_array($in['role'] ?? '', ['admin', 'editor'], true) ? $in['role'] : 'editor';
    if (!$name) $errors['name'] = 'الاسم مطلوب';
    if (!filter_var($email, FILTER_VALIDATE_EMAIL)) $errors['email'] = 'البريد الإلكتروني غير صحيح';
    elseif (DB::value('SELECT id FROM users WHERE email = ? AND id <> ?', [$email, $existing['id'] ?? 0])) {
        $errors['email'] = 'هذا البريد مستخدم لحساب آخر';
    }
    if (!$existing && ($in['password'] ?? '') === '') $errors['password'] = 'كلمة المرور مطلوبة';
    if ($errors) throw new HttpError(422, 'يرجى مراجعة الحقول المظللة', $errors);
    return [
        'name' => $name,
        'email' => $email,
        'role' => $role,
        'is_active' => array_key_exists('is_active', $in) ? (filter_var($in['is_active'], FILTER_VALIDATE_BOOL) ? 1 : 0) : 1,
    ];
}

function notify_new_message(int $id): void
{
    try {
        $to = all_settings()['contact']['notify_email'] ?? null;
        if (!$to) return;
        $m = DB::one('SELECT * FROM messages WHERE id = ?', [$id]);
        $site = config('site_url');
        $kind = $m['kind'] === 'register' ? 'طلب تسجيل جديد' : 'رسالة تواصل جديدة';
        $lines = [
            "$kind من الموقع",
            '',
            "الاسم: {$m['name']}",
            "الجوال: {$m['phone']}",
            $m['email'] ? "البريد: {$m['email']}" : null,
            $m['program'] ? "البرنامج: {$m['program']}" : null,
            $m['body'] ? "\n{$m['body']}" : null,
            '',
            "لوحة التحكم: $site/admin/messages",
        ];
        $subject = '=?UTF-8?B?' . base64_encode("$kind - {$m['name']}") . '?=';
        $host = parse_url((string) $site, PHP_URL_HOST) ?: 'alsmow.com';
        @mail($to, $subject, implode("\n", array_filter($lines, fn($l) => $l !== null)), implode("\r\n", [
            'MIME-Version: 1.0',
            'Content-Type: text/plain; charset=UTF-8',
            "From: =?UTF-8?B?" . base64_encode('موقع معهد السمو') . "?= <no-reply@$host>",
        ]));
    } catch (Throwable $e) {
        error_log('[alsmow-api] mail: ' . $e->getMessage());
    }
}
