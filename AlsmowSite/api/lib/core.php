<?php
declare(strict_types=1);

/* ============================================================
   Core: config, database, HTTP helpers, router.
   ============================================================ */

final class HttpError extends RuntimeException
{
    /** @param array<string,string> $errors field => message */
    public function __construct(public readonly int $status, string $message, public readonly array $errors = [])
    {
        parent::__construct($message);
    }
}

function config(?string $key = null): mixed
{
    static $cfg = null;
    if ($cfg === null) {
        $file = __DIR__ . '/../config.php';
        if (!is_file($file)) throw new HttpError(500, 'الإعدادات غير موجودة على الخادم');
        $cfg = require $file;
    }
    if ($key === null) return $cfg;
    $v = $cfg;
    foreach (explode('.', $key) as $part) $v = $v[$part] ?? null;
    return $v;
}

final class DB
{
    private static ?PDO $pdo = null;

    public static function pdo(): PDO
    {
        if (self::$pdo === null) {
            $c = config('db');
            self::$pdo = new PDO(
                "mysql:host={$c['host']};port=" . ($c['port'] ?? 3306) . ";dbname={$c['name']};charset=utf8mb4",
                $c['user'],
                $c['pass'],
                [
                    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
                    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
                    PDO::ATTR_EMULATE_PREPARES => false,
                    PDO::ATTR_STRINGIFY_FETCHES => false,
                    // Local dev (php -S) reaches the Hostinger DB through an SSH tunnel: keep the connection open
                    // between requests instead of paying ~0.5 s to reconnect every time. Hosting is unchanged.
                    PDO::ATTR_PERSISTENT => PHP_SAPI === 'cli-server',
                    PDO::MYSQL_ATTR_INIT_COMMAND => "SET time_zone = '+03:00'",
                ]
            );
        }
        return self::$pdo;
    }

    public static function run(string $sql, array $params = []): PDOStatement
    {
        $st = self::pdo()->prepare($sql);
        $st->execute($params);
        return $st;
    }

    public static function all(string $sql, array $params = []): array
    {
        return self::run($sql, $params)->fetchAll();
    }

    public static function one(string $sql, array $params = []): ?array
    {
        $row = self::run($sql, $params)->fetch();
        return $row === false ? null : $row;
    }

    public static function value(string $sql, array $params = []): mixed
    {
        $v = self::run($sql, $params)->fetchColumn();
        return $v === false ? null : $v;
    }

    /** Column names come only from server-side whitelists, never from the request. */
    public static function insert(string $table, array $data): int
    {
        $cols = array_keys($data);
        $sql = sprintf(
            'INSERT INTO `%s` (%s) VALUES (%s)',
            $table,
            implode(',', array_map(fn($c) => "`$c`", $cols)),
            implode(',', array_map(fn($c) => ":$c", $cols))
        );
        self::run($sql, $data);
        return (int) self::pdo()->lastInsertId();
    }

    public static function update(string $table, int $id, array $data): void
    {
        if (!$data) return;
        $set = implode(',', array_map(fn($c) => "`$c` = :$c", array_keys($data)));
        $data['__id'] = $id;
        self::run("UPDATE `$table` SET $set WHERE id = :__id", $data);
    }
}

/* ---------------- HTTP ---------------- */

function json_out(mixed $data, int $status = 200, array $headers = []): never
{
    http_response_code($status);
    header('Content-Type: application/json; charset=utf-8');
    foreach ($headers as $k => $v) header("$k: $v");
    echo json_encode($data, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
    exit;
}

function body(): array
{
    static $body = null;
    if ($body === null) {
        $raw = file_get_contents('php://input') ?: '';
        $body = $raw === '' ? [] : json_decode($raw, true);
        if (!is_array($body)) throw new HttpError(400, 'صيغة البيانات غير صحيحة');
    }
    return $body;
}

function client_ip(): string
{
    // Hostinger sits behind its own proxy; REMOTE_ADDR is the real client there.
    return substr((string) ($_SERVER['REMOTE_ADDR'] ?? '0.0.0.0'), 0, 45);
}

/** Simple sliding-window limiter stored in MySQL. Throws 429 when over the limit. */
function rate_limit(string $kind, int $max, int $windowSeconds): void
{
    $ip = client_ip();
    DB::run('DELETE FROM rate_limits WHERE at < (NOW() - INTERVAL 1 DAY)');
    $count = (int) DB::value(
        'SELECT COUNT(*) FROM rate_limits WHERE kind = ? AND ip = ? AND at > (NOW() - INTERVAL ? SECOND)',
        [$kind, $ip, $windowSeconds]
    );
    if ($count >= $max) {
        throw new HttpError(429, 'محاولات كثيرة، يرجى الانتظار قليلًا ثم المحاولة مرة أخرى');
    }
    DB::run('INSERT INTO rate_limits (kind, ip) VALUES (?, ?)', [$kind, $ip]);
}

function clear_rate_limit(string $kind): void
{
    DB::run('DELETE FROM rate_limits WHERE kind = ? AND ip = ?', [$kind, client_ip()]);
}

/* ---------------- Router ---------------- */

final class Router
{
    /** @var array<int, array{0:string,1:string,2:callable}> */
    private array $routes = [];

    public function add(string $method, string $pattern, callable $handler): void
    {
        // "/programs/{id}" -> named regex groups
        $regex = '#^' . preg_replace('#\{(\w+)\}#', '(?P<$1>[^/]+)', $pattern) . '$#u';
        $this->routes[] = [$method, $regex, $handler];
    }

    public function get(string $p, callable $h): void { $this->add('GET', $p, $h); }
    public function post(string $p, callable $h): void { $this->add('POST', $p, $h); }
    public function put(string $p, callable $h): void { $this->add('PUT', $p, $h); }
    public function delete(string $p, callable $h): void { $this->add('DELETE', $p, $h); }

    public function dispatch(string $method, string $path): never
    {
        $allowed = false;
        foreach ($this->routes as [$m, $regex, $handler]) {
            if (!preg_match($regex, $path, $match)) continue;
            $allowed = true;
            if ($m !== $method) continue;
            $params = array_filter($match, 'is_string', ARRAY_FILTER_USE_KEY);
            $params = array_map(fn($v) => rawurldecode($v), $params);
            $result = $handler($params);
            json_out($result ?? ['ok' => true]);
        }
        throw new HttpError($allowed ? 405 : 404, $allowed ? 'طريقة الطلب غير مسموحة' : 'المسار غير موجود');
    }
}

/* ---------------- Helpers ---------------- */

function now_str(): string
{
    return (new DateTimeImmutable('now', new DateTimeZone('Asia/Riyadh')))->format('Y-m-d H:i:s');
}

function make_slug(string $text): string
{
    $s = mb_strtolower(trim($text));
    // keep Arabic + Latin letters and digits, everything else becomes a dash
    $s = preg_replace('/[^\p{Arabic}\p{L}\p{N}]+/u', '-', $s) ?? '';
    $s = trim($s, '-');
    $s = mb_substr($s, 0, 120);
    return $s !== '' ? $s : bin2hex(random_bytes(4));
}

function unique_slug(string $table, string $slug, ?int $exceptId = null): string
{
    $base = $slug;
    $i = 2;
    while (true) {
        $id = DB::value("SELECT id FROM `$table` WHERE slug = ?", [$slug]);
        if ($id === null || ($exceptId !== null && (int) $id === $exceptId)) return $slug;
        $slug = $base . '-' . $i++;
    }
}

function log_activity(?array $user, string $action, string $resource, ?int $id, ?string $label): void
{
    try {
        DB::insert('activity_log', [
            'user_id' => $user['id'] ?? null,
            'action' => $action,
            'resource' => $resource,
            'record_id' => $id,
            'label' => $label !== null ? mb_substr($label, 0, 250) : null,
        ]);
    } catch (Throwable) {
        // logging must never break the request
    }
}
