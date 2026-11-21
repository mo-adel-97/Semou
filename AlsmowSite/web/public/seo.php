<?php
declare(strict_types=1);

/* ============================================================================================================
   Search-engine front door for every page of the site (Apache sends every non-file URL here, see .htaccess).
   It serves the same index.html the app uses, plus what crawlers need to understand the page without running JS:
   the page's own <title>, description, canonical URL, Open Graph / Twitter cards, schema.org JSON-LD
   (the institute, its courses, articles, breadcrumbs) and the page's real text and links inside #root
   (React replaces it on start). Unknown URLs answer 404. /sitemap.xml is generated from the content.
   Any failure falls back to the plain index.html, so the site can never break because of this file.
   ============================================================================================================ */

$INDEX = __DIR__ . '/index.html';

function seo_plain(string $index): never
{
    header('Content-Type: text/html; charset=utf-8');
    header('Cache-Control: no-cache, no-store, must-revalidate');
    readfile($index);
    exit;
}

try {
    $api = __DIR__ . '/api/lib';
    if (!is_file($api . '/core.php') || !is_file(__DIR__ . '/api/config.php')) seo_plain($INDEX);
    require $api . '/core.php';
    require $api . '/content.php';
    require $api . '/resources.php';
    require $api . '/settings.php';
    mb_internal_encoding('UTF-8');

    $S = all_settings();
    $g = $S['general'] ?? [];
    $seo = $S['seo'] ?? [];
    $c = $S['contact'] ?? [];
    $site = rtrim((string) (config('site_url') ?: ('https://' . ($_SERVER['HTTP_HOST'] ?? 'alsmow.com'))), '/');
    $name = trim((string) ($g['site_name'] ?? '')) ?: 'معهد السمو للتدريب';

    $path = '/' . trim(rawurldecode((string) parse_url($_SERVER['REQUEST_URI'] ?? '/', PHP_URL_PATH)), '/');
    if ($path === '/sitemap.xml' || isset($_GET['sitemap'])) seo_sitemap($site);

    $e = fn($s) => htmlspecialchars((string) $s, ENT_QUOTES | ENT_SUBSTITUTE, 'UTF-8');
    $abs = fn(?string $u) => !$u ? null : (preg_match('#^https?://#i', $u) ? $u : $site . '/' . ltrim($u, '/'));
    $clip = function (?string $s, int $n = 160): string {
        $s = trim(preg_replace('/\s+/u', ' ', strip_tags((string) $s)) ?? '');
        return mb_strlen($s) > $n ? rtrim(mb_substr($s, 0, $n - 1)) . '…' : $s;
    };

    $types = [
        'diploma' => ['دبلوم', 'الدبلومات المعتمدة', 'برامج دبلوم معتمدة تؤهلك لسوق العمل بشهادة موثوقة'],
        'qualifying' => ['دورة تأهيلية', 'الدورات التأهيلية', 'دورات تأهيلية تبني مهاراتك الأساسية خطوة بخطوة'],
        'development' => ['دورة تطويرية', 'الدورات التطويرية', 'دورات قصيرة لتطوير مهاراتك ومواكبة الجديد'],
    ];
    $modes = ['onsite' => 'Onsite', 'online' => 'Online', 'hybrid' => 'Blended'];

    $programs = DB::all('SELECT id, type, title, slug, summary, image, hours, duration, mode, accreditation, updated_at FROM programs WHERE is_published = 1 ORDER BY sort ASC, id DESC');
    $siteDesc = trim((string) ($seo['description'] ?? '')) ?: $clip($S['about']['summary'] ?? '');
    $ogDefault = $abs($seo['og_image'] ?? null) ?: $abs($S['hero']['image'] ?? null) ?: $abs($g['logo'] ?? null);

    // ---------- the institute (used on every page) ----------
    $orgId = $site . '/#organization';
    $social = array_values(array_filter(array_map(fn($v) => is_string($v) && preg_match('#^https?://#', $v) ? $v : null, (array) ($S['social'] ?? []))));
    $alt = array_values(array_filter(array_map('trim', (array) ($seo['alt_names'] ?? []))));
    $keywords = array_values(array_filter(array_map('trim', (array) ($seo['keywords'] ?? []))));
    $org = array_filter([
        '@type' => ['EducationalOrganization', 'LocalBusiness'],
        '@id' => $orgId,
        'name' => $name,
        'alternateName' => $alt ?: null,
        'url' => $site . '/',
        'logo' => $abs($g['logo'] ?? null),
        'image' => $ogDefault,
        'description' => $siteDesc ?: null,
        'telephone' => $c['phone'] ?? null,
        'email' => $c['email'] ?? null,
        'address' => [
            '@type' => 'PostalAddress',
            'streetAddress' => $c['address'] ?? null,
            'addressLocality' => 'حفر الباطن',
            'addressRegion' => 'المنطقة الشرقية',
            'addressCountry' => 'SA',
        ],
        'areaServed' => ['@type' => 'Country', 'name' => 'المملكة العربية السعودية'],
        'hasMap' => $c['map_url'] ?? null,
        'sameAs' => $social ?: null,
        'knowsAbout' => $keywords ?: null,
        'slogan' => $g['tagline'] ?? null,
    ], fn($v) => $v !== null && $v !== '' && $v !== []);
    $ld = [$org, [
        '@type' => 'WebSite', '@id' => $site . '/#website', 'url' => $site . '/', 'name' => $name,
        'alternateName' => $alt ?: null, 'inLanguage' => 'ar-SA', 'publisher' => ['@id' => $orgId],
    ]];
    $crumbs = [['الرئيسية', $site . '/']];

    $course = function (array $p) use ($site, $orgId, $abs, $clip, $types, $modes): array {
        return array_filter([
            '@type' => 'Course',
            'name' => $p['title'],
            'description' => $clip($p['summary'] ?? '', 300) ?: $p['title'],
            'url' => $site . '/programs/' . rawurlencode($p['slug']),
            'image' => $abs($p['image'] ?? null),
            'inLanguage' => 'ar',
            'provider' => ['@id' => $orgId],
            'educationalCredentialAwarded' => $types[$p['type']][0] ?? null,
            'timeRequired' => !empty($p['hours']) ? 'PT' . (int) $p['hours'] . 'H' : null,
            'hasCourseInstance' => !empty($p['mode']) ? ['@type' => 'CourseInstance', 'courseMode' => $modes[$p['mode']] ?? null] : null,
        ], fn($v) => $v !== null && $v !== '');
    };

    // ---------- the page ----------
    $status = 200; $type = 'website'; $title = ''; $desc = $siteDesc; $image = $ogDefault; $h1 = ''; $body = '';
    $links = fn(array $items) => '<ul>' . implode('', array_map(fn($p) => '<li><a href="/programs/' . $e(rawurlencode($p['slug'])) . '">' . $e($p['title']) . '</a>' . (!empty($p['summary']) ? ' — ' . $e($clip($p['summary'], 180)) : '') . '</li>', $items)) . '</ul>';
    $seg = array_values(array_filter(explode('/', $path), 'strlen'));

    if (!$seg) {
        $title = trim((string) ($seo['title'] ?? '')) ?: $name . ' | ' . ($g['tagline'] ?? '');
        $h1 = $S['hero']['title'] ?? $name;
        $body = '<p>' . $e($S['hero']['subtitle'] ?? $siteDesc) . '</p>';
        if (!empty($S['hero']['highlights'])) $body .= '<ul>' . implode('', array_map(fn($x) => '<li>' . $e($x) . '</li>', (array) $S['hero']['highlights'])) . '</ul>';
        if (!empty($S['about']['summary'])) $body .= '<h2>' . $e($S['about']['title'] ?? 'عن المعهد') . '</h2><p>' . $e($S['about']['summary']) . '</p>';
        foreach ($types as $k => $t) {
            $list = array_values(array_filter($programs, fn($p) => $p['type'] === $k));
            if ($list) $body .= '<h2><a href="/programs/type/' . $k . '">' . $e($t[1]) . '</a></h2>' . $links($list);
        }
        $ld[] = ['@type' => 'ItemList', 'name' => 'برامج ' . $name, 'itemListElement' => array_map(fn($p, $i) => ['@type' => 'ListItem', 'position' => $i + 1, 'item' => $course($p)], $programs, array_keys($programs))];
    } elseif ($seg[0] === 'about' && count($seg) === 1) {
        $a = $S['about'] ?? [];
        $title = 'عن المعهد';
        $h1 = $a['title'] ?? 'عن ' . $name;
        $desc = $clip($a['summary'] ?? $siteDesc);
        $image = $abs($a['image'] ?? null) ?: $image;
        $body = '<p>' . $e($a['summary'] ?? '') . '</p>' . ($a['body'] ?? '')
            . (!empty($a['vision']) ? '<h2>الرؤية</h2><p>' . $e($a['vision']) . '</p>' : '')
            . (!empty($a['mission']) ? '<h2>الرسالة</h2><p>' . $e($a['mission']) . '</p>' : '');
        $crumbs[] = [$title, $site . '/about'];
    } elseif ($seg[0] === 'programs' && count($seg) === 1) {
        $title = 'برامجنا التدريبية';
        $h1 = $title;
        $desc = 'دبلومات ودورات معتمدة صُممت لتواكب احتياجات سوق العمل وتفتح لك آفاقًا مهنية جديدة.';
        foreach ($types as $k => $t) {
            $list = array_values(array_filter($programs, fn($p) => $p['type'] === $k));
            if ($list) $body .= '<h2><a href="/programs/type/' . $k . '">' . $e($t[1]) . '</a></h2>' . $links($list);
        }
        $crumbs[] = [$title, $site . '/programs'];
    } elseif ($seg[0] === 'programs' && ($seg[1] ?? '') === 'type' && isset($seg[2], $types[$seg[2]]) && count($seg) === 3) {
        [, $plural, $blurb] = $types[$seg[2]];
        $title = $plural; $h1 = $plural; $desc = $blurb;
        $body = '<p>' . $e($blurb) . '</p>' . $links(array_values(array_filter($programs, fn($p) => $p['type'] === $seg[2])));
        $crumbs[] = ['البرامج', $site . '/programs'];
        $crumbs[] = [$plural, $site . '/programs/type/' . $seg[2]];
    } elseif ($seg[0] === 'programs' && count($seg) === 2) {
        $p = DB::one('SELECT * FROM programs WHERE slug = ? AND is_published = 1', [$seg[1]]);
        if ($p) {
            $title = $p['title']; $h1 = $p['title'];
            $desc = $clip($p['summary'] ?: ($p['body'] ?? '')) ?: $siteDesc;
            $image = $abs($p['image']) ?: $image;
            $facts = array_filter([
                'نوع البرنامج' => $types[$p['type']][0] ?? null,
                'المدة' => $p['duration'] ?: null,
                'الساعات المعتمدة' => $p['hours'] ? (string) $p['hours'] : null,
                'جهة الاعتماد' => $p['accreditation'] ?: null,
            ]);
            $body = '<p>' . $e($p['summary'] ?? '') . '</p>'
                . ($facts ? '<ul>' . implode('', array_map(fn($k, $v) => '<li>' . $e($k) . ': ' . $e($v) . '</li>', array_keys($facts), $facts)) . '</ul>' : '')
                . ($p['body'] ?? '');
            $rel = array_values(array_filter($programs, fn($x) => $x['type'] === $p['type'] && (int) $x['id'] !== (int) $p['id']));
            if ($rel) $body .= '<h2>برامج ذات صلة</h2>' . $links(array_slice($rel, 0, 6));
            $ld[] = $course($p);
            $crumbs[] = ['البرامج', $site . '/programs'];
            if (isset($types[$p['type']])) $crumbs[] = [$types[$p['type']][1], $site . '/programs/type/' . $p['type']];
            $crumbs[] = [$p['title'], $site . '/programs/' . rawurlencode($p['slug'])];
        } else $status = 404;
    } elseif ($seg[0] === 'blog' && count($seg) === 1) {
        $title = 'المدونة'; $h1 = 'المدونة والأخبار'; $desc = 'مقالات مفيدة وأحدث أخبار وفعاليات ' . $name . '.';
        $posts = DB::all('SELECT title, slug, excerpt FROM posts WHERE is_published = 1 ORDER BY published_at DESC, id DESC LIMIT 30');
        $body = '<ul>' . implode('', array_map(fn($x) => '<li><a href="/blog/' . $e(rawurlencode($x['slug'])) . '">' . $e($x['title']) . '</a> — ' . $e($clip($x['excerpt'] ?? '', 160)) . '</li>', $posts)) . '</ul>';
        $crumbs[] = ['المدونة', $site . '/blog'];
    } elseif ($seg[0] === 'blog' && count($seg) === 2) {
        $p = DB::one('SELECT * FROM posts WHERE slug = ? AND is_published = 1', [$seg[1]]);
        if ($p) {
            $type = 'article'; $title = $p['title']; $h1 = $p['title'];
            $desc = $clip($p['excerpt'] ?: ($p['body'] ?? '')) ?: $siteDesc;
            $image = $abs($p['cover'] ?? null) ?: $image;
            $body = '<p>' . $e($p['excerpt'] ?? '') . '</p>' . ($p['body'] ?? '');
            $ld[] = array_filter([
                '@type' => 'Article', 'headline' => $clip($p['title'], 110), 'description' => $desc, 'image' => $image,
                'datePublished' => !empty($p['published_at']) ? date('c', strtotime((string) $p['published_at'])) : null,
                'dateModified' => !empty($p['updated_at']) ? date('c', strtotime((string) $p['updated_at'])) : null,
                'author' => ['@id' => $orgId], 'publisher' => ['@id' => $orgId], 'inLanguage' => 'ar',
                'mainEntityOfPage' => $site . '/blog/' . rawurlencode($p['slug']),
            ]);
            $crumbs[] = ['المدونة', $site . '/blog'];
            $crumbs[] = [$p['title'], $site . '/blog/' . rawurlencode($p['slug'])];
        } else $status = 404;
    } elseif ($seg[0] === 'p' && count($seg) === 2) {
        $p = DB::one('SELECT title, slug, body FROM pages WHERE slug = ? AND is_published = 1', [$seg[1]]);
        if ($p) {
            $title = $p['title']; $h1 = $p['title']; $desc = $clip($p['body'] ?? '') ?: $siteDesc; $body = $p['body'] ?? '';
            $crumbs[] = [$p['title'], $site . '/p/' . rawurlencode($p['slug'])];
        } else $status = 404;
    } elseif ($seg[0] === 'contact' && count($seg) === 1) {
        $title = 'تواصل معنا'; $h1 = $title; $desc = 'تواصل مع ' . $name . ': ' . implode(' · ', array_filter([$c['phone'] ?? null, $c['address'] ?? null]));
        $crumbs[] = [$title, $site . '/contact'];
    } elseif ($seg[0] === 'register' && count($seg) === 1) {
        $title = 'التسجيل'; $h1 = 'سجّل اهتمامك الآن'; $desc = 'سجّل اهتمامك في برامج ' . $name . ' وسيتواصل معك فريق القبول.';
        $crumbs[] = [$title, $site . '/register'];
    } elseif ($seg[0] === 'portal' && count($seg) === 1) {
        $title = 'بوابة المتدربين'; $h1 = $title; $desc = 'تابع برامجك وحسابك ومدفوعاتك في ' . $name . '.';
    } else {
        $status = 404;
    }

    if ($status === 404) { $title = 'الصفحة غير موجودة'; $h1 = $title; $body = ''; }
    // contact + places on every page: the strongest local signal, and real text for the crawler
    $contactHtml = '<h2>' . $e($name) . '</h2><ul>'
        . implode('', array_map(fn($b) => '<li>' . $e($b) . '</li>', array_filter(array_map('trim', (array) ($c['branches'] ?? [])))))
        . (!empty($c['address']) ? '<li>العنوان: ' . $e($c['address']) . '</li>' : '')
        . (!empty($c['phone']) ? '<li>الهاتف: <a href="tel:' . $e(preg_replace('/\s+/', '', (string) $c['phone'])) . '">' . $e($c['phone']) . '</a></li>' : '')
        . (!empty($c['email']) ? '<li>البريد: ' . $e($c['email']) . '</li>' : '')
        . '</ul>';
    $nav = '<nav><a href="/">الرئيسية</a> · <a href="/about">عن المعهد</a> · <a href="/programs">البرامج</a> · '
        . implode(' · ', array_map(fn($k, $t) => '<a href="/programs/type/' . $k . '">' . $e($t[1]) . '</a>', array_keys($types), $types))
        . ' · <a href="/blog">المدونة</a> · <a href="/contact">تواصل معنا</a> · <a href="/register">التسجيل</a></nav>';

    $fullTitle = $seg && $status === 200 ? $title . ' | ' . $name : ($status === 404 ? $title . ' | ' . $name : $title);
    $canonical = $site . ($seg ? '/' . implode('/', array_map('rawurlencode', $seg)) : '/');
    if (count($crumbs) > 1 && $status === 200) {
        $ld[] = ['@type' => 'BreadcrumbList', 'itemListElement' => array_map(fn($x, $i) => ['@type' => 'ListItem', 'position' => $i + 1, 'name' => $x[0], 'item' => $x[1]], $crumbs, array_keys($crumbs))];
    }

    // ---------- head ----------
    $head = '<title data-ssr>' . $e($fullTitle) . '</title>'
        . '<meta data-ssr name="description" content="' . $e($desc) . '" />'
        . ($keywords ? '<meta data-ssr name="keywords" content="' . $e(implode('، ', $keywords)) . '" />' : '')
        . '<meta data-ssr name="robots" content="' . ($status === 200 ? 'index, follow, max-image-preview:large' : 'noindex, follow') . '" />'
        . ($status === 200 ? '<link data-ssr rel="canonical" href="' . $e($canonical) . '" />' : '')
        . '<meta data-ssr property="og:site_name" content="' . $e($name) . '" />'
        . '<meta data-ssr property="og:locale" content="ar_SA" />'
        . '<meta data-ssr property="og:type" content="' . $type . '" />'
        . '<meta data-ssr property="og:title" content="' . $e($fullTitle) . '" />'
        . '<meta data-ssr property="og:description" content="' . $e($desc) . '" />'
        . '<meta data-ssr property="og:url" content="' . $e($canonical) . '" />'
        . ($image ? '<meta data-ssr property="og:image" content="' . $e($image) . '" />' : '')
        . '<meta data-ssr name="twitter:card" content="summary_large_image" />'
        . '<meta data-ssr name="twitter:title" content="' . $e($fullTitle) . '" />'
        . '<meta data-ssr name="twitter:description" content="' . $e($desc) . '" />'
        . ($image ? '<meta data-ssr name="twitter:image" content="' . $e($image) . '" />' : '')
        . (!empty($seo['google_verify']) ? '<meta name="google-site-verification" content="' . $e($seo['google_verify']) . '" />' : '')
        . (!empty($seo['bing_verify']) ? '<meta name="msvalidate.01" content="' . $e($seo['bing_verify']) . '" />' : '')
        . '<script type="application/ld+json">' . json_encode(['@context' => 'https://schema.org', '@graph' => array_values($ld)], JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_HEX_TAG) . '</script>';

    // the page's real text and links for crawlers; visually hidden, React replaces it as soon as it starts
    $ssr = '<div class="seo-ssr" style="position:absolute;width:1px;height:1px;overflow:hidden;clip:rect(0 0 0 0);white-space:normal">'
        . $nav . '<main><h1>' . $e($h1) . '</h1>' . $body . '</main>' . $contactHtml . '</div>';

    $html = (string) file_get_contents($INDEX);
    // drop index.html's generic tags that this page replaces
    $html = preg_replace(['#<title>.*?</title>#su', '#<meta\s+name="description"[^>]*>#iu', '#<meta\s+property="og:[^"]+"[^>]*>#iu', '#<meta\s+name="twitter:[^"]+"[^>]*>#iu'], '', $html) ?? $html;
    $html = str_replace('</head>', $head . '</head>', $html);
    $html = preg_replace('#<div id="root">\s*</div>#', '<div id="root">' . str_replace(['\\', '$'], ['\\\\', '\\$'], $ssr) . '</div>', $html, 1) ?? $html;

    http_response_code($status);
    header('Content-Type: text/html; charset=utf-8');
    header('Cache-Control: no-cache, no-store, must-revalidate');
    echo $html;
} catch (Throwable $err) {
    error_log('[alsmow-seo] ' . $err);
    seo_plain($INDEX);
}

/** /sitemap.xml: every public page, programme, article and custom page, with last-modified dates. */
function seo_sitemap(string $site): never
{
    $urls = [['/', null, '1.0'], ['/programs', null, '0.9'], ['/programs/type/diploma', null, '0.9'], ['/programs/type/qualifying', null, '0.8'],
        ['/programs/type/development', null, '0.8'], ['/about', null, '0.7'], ['/contact', null, '0.7'], ['/register', null, '0.7'], ['/blog', null, '0.6']];
    foreach (DB::all('SELECT slug, updated_at FROM programs WHERE is_published = 1') as $r) $urls[] = ['/programs/' . rawurlencode($r['slug']), $r['updated_at'], '0.9'];
    foreach (DB::all('SELECT slug, updated_at FROM posts WHERE is_published = 1') as $r) $urls[] = ['/blog/' . rawurlencode($r['slug']), $r['updated_at'], '0.6'];
    foreach (DB::all('SELECT slug, updated_at FROM pages WHERE is_published = 1') as $r) $urls[] = ['/p/' . rawurlencode($r['slug']), $r['updated_at'], '0.4'];
    header('Content-Type: application/xml; charset=utf-8');
    header('Cache-Control: public, max-age=3600');
    echo '<?xml version="1.0" encoding="UTF-8"?>' . "\n" . '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">' . "\n";
    foreach ($urls as [$u, $mod, $prio]) {
        echo '  <url><loc>' . htmlspecialchars($site . $u, ENT_XML1) . '</loc>'
            . ($mod ? '<lastmod>' . date('Y-m-d', strtotime((string) $mod)) . '</lastmod>' : '')
            . '<priority>' . $prio . '</priority></url>' . "\n";
    }
    echo '</urlset>';
    exit;
}
