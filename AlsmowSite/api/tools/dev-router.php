<?php
// Local only: `php -S 127.0.0.1:8090 api/tools/dev-router.php` from the project root.
// Mimics the .htaccess rules: /api/* -> api/index.php, /uploads/* -> static files.
$path = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);
if (str_starts_with($path, '/uploads/')) {
    $file = realpath(__DIR__ . '/../..' . $path);
    $root = realpath(__DIR__ . '/../../uploads');
    if ($file && $root && str_starts_with($file, $root) && is_file($file)) {
        header('Content-Type: ' . (mime_content_type($file) ?: 'application/octet-stream'));
        readfile($file);
        return true;
    }
    http_response_code(404);
    return true;
}
if (str_starts_with($path, '/api')) {
    require __DIR__ . '/../index.php';
    return true;
}
http_response_code(404);
echo 'dev router: only /api and /uploads';
return true;
