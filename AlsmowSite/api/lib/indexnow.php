<?php
declare(strict_types=1);

/* ============================================================================================================
   IndexNow: tells Bing, Yandex, Seznam … (one call reaches them all) that a page changed, so a programme or article
   saved in the dashboard is crawled within minutes instead of days. The key is public by design: the same value is
   served as /<key>.txt from the site root (web/public), which proves the site owns it.
   ============================================================================================================ */

const INDEXNOW_KEY = '8fb8471bcba250ddd762d2f78fa05831';

/** The public URL of a saved record (programmes, articles, pages), or null for content with no page of its own. */
function indexnow_url(string $res, ?array $row): ?string
{
    if (!$row || empty($row['slug'])) return null;
    $base = ['programs' => '/programs/', 'posts' => '/blog/', 'pages' => '/p/'][$res] ?? null;
    return $base ? $base . rawurlencode((string) $row['slug']) : null;
}

/** Sends the changed URLs (plus the lists that show them) after the response has gone out. Never throws. */
function indexnow_ping(string $res, array $paths): void
{
    $paths = array_values(array_unique(array_filter($paths)));
    if (!$paths || PHP_SAPI === 'cli-server') return;            // never from a local run
    $site = rtrim((string) (config('site_url') ?: ''), '/');
    $host = (string) parse_url($site, PHP_URL_HOST);
    if ($host === '' || $host === 'localhost' || str_starts_with($host, '127.')) return;
    $lists = ['programs' => ['/', '/programs', '/programs/type/diploma', '/programs/type/qualifying', '/programs/type/development'], 'posts' => ['/blog']][$res] ?? [];
    $urls = array_map(fn($p) => $site . $p, array_values(array_unique([...$paths, ...$lists])));
    register_shutdown_function(function () use ($host, $site, $urls) {
        try {
            if (function_exists('fastcgi_finish_request')) fastcgi_finish_request();
            $ctx = stream_context_create(['http' => [
                'method' => 'POST', 'timeout' => 5, 'ignore_errors' => true,
                'header' => "Content-Type: application/json; charset=utf-8\r\n",
                'content' => json_encode(['host' => $host, 'key' => INDEXNOW_KEY, 'keyLocation' => $site . '/' . INDEXNOW_KEY . '.txt', 'urlList' => $urls], JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE),
            ]]);
            @file_get_contents('https://api.indexnow.org/indexnow', false, $ctx);
        } catch (Throwable $e) {
            error_log('[alsmow-indexnow] ' . $e->getMessage());
        }
    });
}
