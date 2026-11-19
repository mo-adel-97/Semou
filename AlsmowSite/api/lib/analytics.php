<?php
declare(strict_types=1);

/* ============================================================
   Site analytics, self-hosted and privacy-friendly:
   - no cookies, no IP stored: a visitor is a hash of (ip, user agent, day, secret) → unique per day;
   - the browser sends page views, time on page and a few events (web/src/lib/track.ts);
   - bots, headless browsers and signed-in staff are not counted;
   - the dashboard reads one aggregated report for any date range, compared with the period before.
   ============================================================ */

const AN_EVENTS = ['chat_open', 'chat_message', 'register_click', 'register_submit', 'contact_submit', 'whatsapp_click', 'phone_click', 'email_click', 'portal_click', 'program_view', 'theme_change', 'share'];

function an_ensure_table(): void
{
    static $done = false;
    if ($done) return;
    DB::pdo()->exec((string) file_get_contents(__DIR__ . '/../migrations/003_analytics.sql'));
    $done = true;
}

function an_ua(string $ua): array
{
    $u = strtolower($ua);
    $device = preg_match('/ipad|tablet|(android(?!.*mobile))/', $u) ? 'tablet' : (preg_match('/mobi|iphone|android/', $u) ? 'mobile' : 'desktop');
    $browser = match (true) {
        str_contains($u, 'edg/') => 'Edge',
        str_contains($u, 'opr/') || str_contains($u, 'opera') => 'Opera',
        str_contains($u, 'samsungbrowser') => 'Samsung',
        str_contains($u, 'snapchat') => 'Snapchat',
        str_contains($u, 'instagram') => 'Instagram',
        str_contains($u, 'fban') || str_contains($u, 'fbav') => 'Facebook',
        str_contains($u, 'tiktok') || str_contains($u, 'musical_ly') => 'TikTok',
        str_contains($u, 'firefox') || str_contains($u, 'fxios') => 'Firefox',
        str_contains($u, 'crios') || str_contains($u, 'chrome') => 'Chrome',
        str_contains($u, 'safari') => 'Safari',
        default => 'أخرى',
    };
    $os = match (true) {
        str_contains($u, 'iphone') || str_contains($u, 'ipad') || str_contains($u, 'ios') => 'iOS',
        str_contains($u, 'android') => 'Android',
        str_contains($u, 'windows') => 'Windows',
        str_contains($u, 'mac os') || str_contains($u, 'macintosh') => 'macOS',
        str_contains($u, 'linux') => 'Linux',
        default => 'أخرى',
    };
    return [$device, $browser, $os];
}

function an_is_bot(string $ua): bool
{
    return $ua === '' || (bool) preg_match('/bot|crawl|spider|slurp|headless|lighthouse|preview|facebookexternalhit|whatsapp\/|telegrambot|python|curl|wget|httpclient|phantom|puppeteer|playwright/i', $ua);
}

function an_source(?string $refHost, ?string $utmSource): string
{
    if ($utmSource) return 'campaign';
    if (!$refHost) return 'direct';
    $h = strtolower($refHost);
    if (preg_match('/google\.|bing\.|yahoo\.|duckduckgo|yandex|baidu|ecosia/', $h)) return 'search';
    if (preg_match('/instagram|snapchat|tiktok|facebook|fb\.|t\.co|twitter|x\.com|linkedin|youtube|whatsapp|wa\.me|telegram|t\.me|pinterest|threads/', $h)) return 'social';
    return 'referral';
}

/** Records one hit from the browser. Silent on bad input: analytics must never break the site. */
function an_track(array $in): void
{
    $ua = (string) ($_SERVER['HTTP_USER_AGENT'] ?? '');
    if (an_is_bot($ua) || !empty($in['staff'])) return;
    $kind = in_array($in['kind'] ?? '', ['view', 'leave', 'event'], true) ? $in['kind'] : null;
    if (!$kind) return;
    $name = $kind === 'event' && in_array($in['name'] ?? '', AN_EVENTS, true) ? $in['name'] : null;
    if ($kind === 'event' && !$name) return;
    $session = preg_replace('/[^a-z0-9]/i', '', (string) ($in['sid'] ?? ''));
    if (strlen($session) < 8) return;

    $path = mb_substr('/' . ltrim((string) parse_url((string) ($in['path'] ?? '/'), PHP_URL_PATH), '/'), 0, 300);
    $ref = (string) ($in['ref'] ?? '');
    $refHost = $ref !== '' ? (parse_url($ref, PHP_URL_HOST) ?: null) : null;
    $own = parse_url((string) config('site_url'), PHP_URL_HOST);
    if ($refHost && ($refHost === $own || $refHost === ($_SERVER['HTTP_HOST'] ?? ''))) $refHost = null;
    $refHost = $refHost ? mb_substr(preg_replace('/^www\./', '', strtolower($refHost)), 0, 120) : null;
    $utm = fn(string $k, int $n) => ($v = trim((string) ($in[$k] ?? ''))) !== '' ? mb_substr(strip_tags($v), 0, $n) : null;
    [$device, $browser, $os] = an_ua($ua);
    $day = date('Y-m-d');
    $visitor = substr(hash('sha256', client_ip() . '|' . $ua . '|' . $day . '|' . (string) config('jwt_secret')), 0, 16);
    $country = strtoupper(substr((string) ($_SERVER['HTTP_CF_IPCOUNTRY'] ?? ''), 0, 2)) ?: null;
    $lang = ($l = (string) ($in['lang'] ?? '')) !== '' ? mb_substr(strtolower($l), 0, 10) : null;

    $row = [
        date('Y-m-d H:i:s'), $day, $kind, $name, $path,
        ($t = trim((string) ($in['title'] ?? ''))) !== '' ? mb_substr(strip_tags($t), 0, 200) : null,
        $kind === 'view' ? $refHost : null,
        $kind === 'view' ? an_source($refHost, $utm('utm_source', 80)) : null,
        $kind === 'view' ? $utm('utm_source', 80) : null,
        $kind === 'view' ? $utm('utm_medium', 80) : null,
        $kind === 'view' ? $utm('utm_campaign', 120) : null,
        $visitor, substr($session, 0, 16),
        !empty($in['new']) ? 1 : 0, !empty($in['entry']) ? 1 : 0,
        $device, $browser, $os, $lang, $country,
        ($w = (int) ($in['w'] ?? 0)) > 0 ? min($w, 9999) : null,
        $kind === 'leave' ? max(0, min(3600, (int) ($in['dur'] ?? 0))) : null,
    ];
    $sql = 'INSERT INTO analytics_events (at, day, kind, name, path, title, ref_host, source, utm_source, utm_medium, utm_campaign, visitor, session, is_new, entry, device, browser, os, lang, country, screen_w, duration) VALUES (' . implode(',', array_fill(0, count($row), '?')) . ')';
    try {
        DB::run($sql, $row);
    } catch (PDOException $e) {
        if (($e->errorInfo[1] ?? 0) !== 1146) throw $e; // table missing: create it once, then retry
        an_ensure_table();
        DB::run($sql, $row);
    }
    // keep ~13 months
    if (random_int(1, 500) === 1) DB::run('DELETE FROM analytics_events WHERE at < (NOW() - INTERVAL 400 DAY)');
}

/* ---------- report ---------- */

function an_range(string $from, string $to): array
{
    $tz = new DateTimeZone('Asia/Riyadh');
    $f = DateTime::createFromFormat('Y-m-d', $from, $tz) ?: new DateTime('-29 days', $tz);
    $t = DateTime::createFromFormat('Y-m-d', $to, $tz) ?: new DateTime('now', $tz);
    if ($f > $t) [$f, $t] = [$t, $f];
    $days = (int) $f->diff($t)->days + 1;
    if ($days > 400) { $f = (clone $t)->modify('-399 days'); $days = 400; }
    $pf = (clone $f)->modify("-$days days");
    $pt = (clone $f)->modify('-1 day');
    return [$f->format('Y-m-d'), $t->format('Y-m-d'), $pf->format('Y-m-d'), $pt->format('Y-m-d'), $days];
}

function an_totals(string $from, string $to): array
{
    $w = 'day BETWEEN ? AND ?';
    $p = [$from, $to];
    $views = (int) DB::value("SELECT COUNT(*) FROM analytics_events WHERE $w AND kind = 'view'", $p);
    $visitors = (int) DB::value("SELECT COUNT(*) FROM (SELECT DISTINCT day, visitor FROM analytics_events WHERE $w AND kind = 'view') x", $p);
    $sess = DB::one(
        "SELECT COUNT(*) sessions, SUM(v = 1) bounces, AVG(v) per_session
         FROM (SELECT session, COUNT(*) v FROM analytics_events WHERE $w AND kind = 'view' GROUP BY session) s",
        $p
    ) ?? [];
    $dur = (float) (DB::value(
        "SELECT AVG(t) FROM (SELECT session, SUM(duration) t FROM analytics_events WHERE $w AND kind = 'leave' GROUP BY session) s",
        $p
    ) ?? 0);
    $newV = (int) DB::value("SELECT COUNT(DISTINCT session) FROM analytics_events WHERE $w AND kind = 'view' AND is_new = 1", $p);
    $sessions = (int) ($sess['sessions'] ?? 0);
    return [
        'visitors' => $visitors,
        'views' => $views,
        'sessions' => $sessions,
        'bounce' => $sessions ? round(((int) $sess['bounces']) / $sessions * 100, 1) : 0,
        'perSession' => round((float) ($sess['per_session'] ?? 0), 2),
        'avgDuration' => (int) round($dur),
        'newShare' => $sessions ? round($newV / $sessions * 100, 1) : 0,
    ];
}

/** @return list<array{k:string,n:int}> */
function an_top(string $col, string $from, string $to, string $extra = '', int $limit = 10, string $count = 'COUNT(*)'): array
{
    return array_map(
        fn($r) => ['k' => (string) ($r['k'] ?? ''), 'n' => (int) $r['n']] + (isset($r['v']) ? ['v' => (int) $r['v']] : []) + (isset($r['d']) ? ['d' => (int) round((float) $r['d'])] : []),
        DB::all("SELECT $col k, $count n FROM analytics_events WHERE day BETWEEN ? AND ? $extra GROUP BY k ORDER BY n DESC LIMIT $limit", [$from, $to])
    );
}

function an_report(string $from, string $to): array
{
    [$from, $to, $pf, $pt, $days] = an_range($from, $to);
    try {
        DB::value('SELECT 1 FROM analytics_events LIMIT 1');
    } catch (PDOException) {
        an_ensure_table();
    }
    $p = [$from, $to];
    $hourly = $days <= 2;

    // time series (filled with zeros)
    $bucket = $hourly ? "DATE_FORMAT(at, '%Y-%m-%d %H:00')" : 'day';
    $rows = DB::all(
        "SELECT $bucket b, SUM(kind = 'view') views, COUNT(DISTINCT CASE WHEN kind = 'view' THEN CONCAT(day, visitor) END) visitors
         FROM analytics_events WHERE day BETWEEN ? AND ? GROUP BY b ORDER BY b",
        $p
    );
    $byB = [];
    foreach ($rows as $r) $byB[(string) $r['b']] = $r;
    $series = [];
    $cur = new DateTime("$from 00:00");
    $end = new DateTime("$to 23:59");
    while ($cur <= $end) {
        $k = $hourly ? $cur->format('Y-m-d H:00') : $cur->format('Y-m-d');
        $series[] = ['t' => $k, 'views' => (int) ($byB[$k]['views'] ?? 0), 'visitors' => (int) ($byB[$k]['visitors'] ?? 0)];
        $cur->modify($hourly ? '+1 hour' : '+1 day');
    }

    // weekday × hour heat map (visitors' local time = Riyadh)
    $heat = array_fill(0, 7, array_fill(0, 24, 0));
    foreach (DB::all("SELECT DAYOFWEEK(at) - 1 d, HOUR(at) h, COUNT(*) n FROM analytics_events WHERE day BETWEEN ? AND ? AND kind = 'view' GROUP BY d, h", $p) as $r) {
        $heat[(int) $r['d']][(int) $r['h']] = (int) $r['n'];
    }

    // funnel: sessions → saw a program → reached /register → sent the form
    $funnel = DB::one(
        "SELECT COUNT(DISTINCT session) s,
                COUNT(DISTINCT CASE WHEN kind = 'view' AND path LIKE '/programs/%' THEN session END) p,
                COUNT(DISTINCT CASE WHEN (kind = 'view' AND path = '/register') OR (kind = 'event' AND name = 'register_click') THEN session END) r,
                COUNT(DISTINCT CASE WHEN kind = 'event' AND name = 'register_submit' THEN session END) d
         FROM analytics_events WHERE day BETWEEN ? AND ?",
        $p
    ) ?? [];
    $regs = 0;
    try { $regs = (int) DB::value("SELECT COUNT(*) FROM messages WHERE kind = 'register' AND DATE(created_at) BETWEEN ? AND ?", $p); } catch (Throwable) {}

    $live = DB::all(
        "SELECT path, COUNT(DISTINCT session) n FROM analytics_events WHERE at >= (NOW() - INTERVAL 5 MINUTE) GROUP BY path ORDER BY n DESC LIMIT 6"
    );

    $pageDur = [];
    foreach (DB::all("SELECT path, AVG(duration) d FROM analytics_events WHERE day BETWEEN ? AND ? AND kind = 'leave' GROUP BY path", $p) as $r) $pageDur[$r['path']] = (int) round((float) $r['d']);
    $pages = an_top('path', $from, $to, "AND kind = 'view'", 12, 'COUNT(*)');
    $titles = [];
    if ($pages) {
        $in = implode(',', array_fill(0, count($pages), '?'));
        foreach (DB::all("SELECT path, MAX(title) t FROM analytics_events WHERE kind = 'view' AND path IN ($in) GROUP BY path", array_column($pages, 'k')) as $r) $titles[$r['path']] = (string) $r['t'];
    }
    foreach ($pages as &$pg) {
        $pg['d'] = $pageDur[$pg['k']] ?? 0;
        $pg['title'] = $titles[$pg['k']] ?? '';
    }
    unset($pg);

    $chatQ = 0;
    try { $chatQ = count(json_decode((string) (DB::value("SELECT v FROM settings WHERE k = 'assistant_unanswered'") ?? '[]'), true) ?: []); } catch (Throwable) {}

    return [
        'range' => ['from' => $from, 'to' => $to, 'days' => $days, 'hourly' => $hourly, 'prevFrom' => $pf, 'prevTo' => $pt],
        'totals' => an_totals($from, $to),
        'prev' => an_totals($pf, $pt),
        'series' => $series,
        'live' => ['total' => (int) DB::value('SELECT COUNT(DISTINCT session) FROM analytics_events WHERE at >= (NOW() - INTERVAL 5 MINUTE)'), 'pages' => array_map(fn($r) => ['k' => $r['path'], 'n' => (int) $r['n']], $live)],
        'pages' => $pages,
        'entries' => an_top('path', $from, $to, "AND kind = 'view' AND entry = 1", 8),
        'sources' => an_top('source', $from, $to, "AND kind = 'view' AND entry = 1", 6),
        'referrers' => an_top('ref_host', $from, $to, "AND kind = 'view' AND ref_host IS NOT NULL", 10),
        'campaigns' => an_top("CONCAT_WS(' / ', utm_source, utm_medium, utm_campaign)", $from, $to, "AND kind = 'view' AND utm_source IS NOT NULL", 10),
        'devices' => an_top('device', $from, $to, "AND kind = 'view' AND entry = 1", 3),
        'browsers' => an_top('browser', $from, $to, "AND kind = 'view' AND entry = 1", 8),
        'os' => an_top('os', $from, $to, "AND kind = 'view' AND entry = 1", 6),
        'langs' => an_top('lang', $from, $to, "AND kind = 'view' AND entry = 1 AND lang IS NOT NULL", 6),
        'countries' => an_top('country', $from, $to, "AND kind = 'view' AND entry = 1 AND country IS NOT NULL", 8),
        'events' => an_top('name', $from, $to, "AND kind = 'event'", 12),
        'heat' => $heat,
        'funnel' => ['sessions' => (int) ($funnel['s'] ?? 0), 'program' => (int) ($funnel['p'] ?? 0), 'register' => (int) ($funnel['r'] ?? 0), 'submitted' => (int) ($funnel['d'] ?? 0), 'registrations' => $regs],
        'chat' => ['unanswered' => $chatQ],
    ];
}
