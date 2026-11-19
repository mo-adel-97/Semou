<?php
// Copy to config.php on the server (never commit config.php).
return [
    'db' => [
        'host' => '127.0.0.1',
        'name' => 'u920789781_xxxx',
        'user' => 'u920789781_xxxx',
        'pass' => '',
    ],
    // 64 random hex chars: php -r "echo bin2hex(random_bytes(32));"
    'jwt_secret' => '',
    'site_url' => 'https://new.alsmow.com',
    'uploads_dir' => __DIR__ . '/../uploads',
    'uploads_url' => '/uploads',
    'cookie_secure' => true,
];
