<?php
declare(strict_types=1);

/* ============================================================
   Auth: HS256 JWT kept in an httpOnly, SameSite=Strict cookie.
   JavaScript never sees the token. Every state-changing admin
   request must also carry "X-Requested-With: fetch", which a
   cross-site form cannot send (CSRF protection).
   ============================================================ */

const AUTH_COOKIE = 'alsmow_session';
const AUTH_HOURS = 12;

function b64url(string $s): string { return rtrim(strtr(base64_encode($s), '+/', '-_'), '='); }
function b64url_dec(string $s): string { return base64_decode(strtr($s, '-_', '+/')) ?: ''; }

function jwt_sign(array $claims): string
{
    $h = b64url(json_encode(['alg' => 'HS256', 'typ' => 'JWT']));
    $p = b64url(json_encode($claims));
    $sig = b64url(hash_hmac('sha256', "$h.$p", config('jwt_secret'), true));
    return "$h.$p.$sig";
}

function jwt_verify(string $token): ?array
{
    $parts = explode('.', $token);
    if (count($parts) !== 3) return null;
    [$h, $p, $sig] = $parts;
    $expected = b64url(hash_hmac('sha256', "$h.$p", config('jwt_secret'), true));
    if (!hash_equals($expected, $sig)) return null;
    $claims = json_decode(b64url_dec($p), true);
    if (!is_array($claims) || ($claims['exp'] ?? 0) < time()) return null;
    return $claims;
}

function set_auth_cookie(string $value, int $expires): void
{
    setcookie(AUTH_COOKIE, $value, [
        'expires' => $expires,
        'path' => '/api',
        'secure' => (bool) config('cookie_secure'),
        'httponly' => true,
        'samesite' => 'Strict',
    ]);
}

function public_user(array $u): array
{
    return [
        'id' => (int) $u['id'],
        'name' => $u['name'],
        'email' => $u['email'],
        'role' => $u['role'],
    ];
}

function current_user(): ?array
{
    static $resolved = false, $user = null;
    if ($resolved) return $user;
    $resolved = true;
    $claims = jwt_verify((string) ($_COOKIE[AUTH_COOKIE] ?? ''));
    if (!$claims) return null;
    $u = DB::one('SELECT * FROM users WHERE id = ? AND is_active = 1', [(int) $claims['sub']]);
    // token_version lets "log out everywhere" / password change revoke old sessions.
    if (!$u || (int) $u['token_version'] !== (int) ($claims['ver'] ?? 0)) return null;
    return $user = $u;
}

/** @param 'admin'|'editor' $role minimum role */
function require_user(string $role = 'editor'): array
{
    $u = current_user();
    if (!$u) throw new HttpError(401, 'انتهت الجلسة، يرجى تسجيل الدخول مرة أخرى');
    if ($role === 'admin' && $u['role'] !== 'admin') throw new HttpError(403, 'هذه العملية متاحة للمدير فقط');
    return $u;
}

function require_csrf_header(): void
{
    if (($_SERVER['HTTP_X_REQUESTED_WITH'] ?? '') !== 'fetch') {
        throw new HttpError(403, 'طلب غير مسموح');
    }
}

function login(string $email, string $password): array
{
    rate_limit('login', 8, 15 * 60);
    $u = DB::one('SELECT * FROM users WHERE email = ?', [mb_strtolower(trim($email))]);
    // Always run password_verify so timing doesn't reveal which emails exist.
    $hash = $u['password_hash'] ?? password_hash(random_bytes(12), PASSWORD_DEFAULT);
    $ok = password_verify($password, $hash);
    if (!$u || !$ok) throw new HttpError(401, 'البريد الإلكتروني أو كلمة المرور غير صحيحة');
    if (!(int) $u['is_active']) throw new HttpError(403, 'هذا الحساب موقوف، تواصل مع مدير الموقع');

    clear_rate_limit('login');
    if (password_needs_rehash($u['password_hash'], PASSWORD_DEFAULT)) {
        DB::update('users', (int) $u['id'], ['password_hash' => password_hash($password, PASSWORD_DEFAULT)]);
    }
    DB::update('users', (int) $u['id'], ['last_login_at' => now_str()]);

    $exp = time() + AUTH_HOURS * 3600;
    set_auth_cookie(jwt_sign(['sub' => (int) $u['id'], 'ver' => (int) $u['token_version'], 'exp' => $exp]), $exp);
    return public_user($u);
}

function logout(): void
{
    set_auth_cookie('', time() - 3600);
}

function validate_password(string $pw): void
{
    if (mb_strlen($pw) < 10) {
        throw new HttpError(422, 'تحقق من البيانات', ['password' => 'كلمة المرور يجب ألا تقل عن 10 أحرف']);
    }
}
