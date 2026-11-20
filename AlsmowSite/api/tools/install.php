<?php
declare(strict_types=1);

/* ============================================================
   CLI tool (SSH only, blocked from the web by .htaccess + check below)

   php api/tools/install.php migrate
   php api/tools/install.php seed
   ADMIN_PASSWORD=... php api/tools/install.php admin <email> "<name>"
   php api/tools/install.php import-wp <path/to/wp-config.php> <path/to/wp-content/uploads>
   ============================================================ */

if (PHP_SAPI !== 'cli') { http_response_code(404); exit; }

require __DIR__ . '/../lib/core.php';
require __DIR__ . '/../lib/auth.php';
require __DIR__ . '/../lib/content.php';
require __DIR__ . '/../lib/resources.php';
require __DIR__ . '/../lib/settings.php';
date_default_timezone_set('Asia/Riyadh');
mb_internal_encoding('UTF-8');

$cmd = $argv[1] ?? '';
try {
    match ($cmd) {
        'migrate' => migrate(),
        'seed' => seed(),
        'admin' => create_admin($argv[2] ?? '', $argv[3] ?? 'مدير الموقع'),
        'import-wp' => import_wp($argv[2] ?? '', $argv[3] ?? ''),
        default => fwrite(STDERR, "usage: install.php migrate|seed|admin|import-wp\n"),
    };
} catch (HttpError $e) {
    fwrite(STDERR, "ERROR: {$e->getMessage()} " . json_encode($e->errors, JSON_UNESCAPED_UNICODE) . "\n");
    exit(1);
}

function out(string $s): void { echo $s, "\n"; }

function migrate(): void
{
    $pdo = DB::pdo();
    $pdo->exec('CREATE TABLE IF NOT EXISTS schema_migrations (name VARCHAR(190) PRIMARY KEY, applied_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP)');
    $done = array_column(DB::all('SELECT name FROM schema_migrations'), 'name');
    $files = glob(__DIR__ . '/../migrations/*.sql');
    sort($files);
    foreach ($files as $f) {
        $name = basename($f);
        if (in_array($name, $done, true)) continue;
        $sql = file_get_contents($f);
        foreach (array_filter(array_map('trim', preg_split('/;\s*\n/', $sql))) as $stmt) {
            if ($stmt === '' || str_starts_with($stmt, '--') && !str_contains($stmt, "\n")) continue;
            $pdo->exec($stmt);
        }
        DB::insert('schema_migrations', ['name' => $name]);
        out("applied $name");
    }
    out('migrations up to date');
}

function create_admin(string $email, string $name): void
{
    $pw = getenv('ADMIN_PASSWORD') ?: '';
    if (!filter_var($email, FILTER_VALIDATE_EMAIL)) throw new HttpError(422, 'invalid email');
    validate_password($pw);
    $email = mb_strtolower($email);
    $id = DB::value('SELECT id FROM users WHERE email = ?', [$email]);
    $hash = password_hash($pw, PASSWORD_DEFAULT);
    if ($id) {
        DB::run('UPDATE users SET password_hash = ?, role = "admin", is_active = 1, token_version = token_version + 1 WHERE id = ?', [$hash, $id]);
        out("admin updated: $email");
    } else {
        DB::insert('users', ['name' => $name, 'email' => $email, 'password_hash' => $hash, 'role' => 'admin']);
        out("admin created: $email");
    }
}

/** Default content. Only fills groups/tables that are still empty. */
function seed(): void
{
    $defaults = [
        'general' => [
            'site_name' => 'معهد السمو للتدريب',
            'tagline' => 'مستقبلك يبدأ من هنا',
            'announcement' => 'التسجيل مفتوح الآن في الدبلومات والدورات المعتمدة',
            'announcement_link' => '/register',
        ],
        'hero' => [
            'eyebrow' => 'معهد معتمد في حفر الباطن',
            'title' => 'ارتقي بطموحك مع معهد السمو',
            'highlight' => 'السمو',
            'subtitle' => 'وجهتك للدورات والدبلومات المعتمدة التي تمكّنك من التميز في سوق العمل، في بيئة تدريبية داعمة مع مدربين مؤهلين وبرامج تواكب احتياجات سوق العمل.',
            'cta_text' => 'ابدأ رحلتك الآن',
            'cta_link' => '/register',
            'secondary_text' => 'استعرض البرامج',
            'secondary_link' => '/programs',
            'highlights' => ['شهادات معتمدة', 'بيئة تدريبية داعمة', 'مدربين مؤهلين', 'برامج تواكب سوق العمل'],
        ],
        'about' => [
            'title' => 'عن معهد السمو',
            'summary' => 'معهد السمو للتدريب مؤسسة تدريبية رائدة في حفر الباطن، تعمل تحت إشراف المؤسسة العامة للتدريب التقني والمهني، وتقدم برامج معتمدة في الإدارة وتقنية المعلومات واللغة الإنجليزية، تمنح خريجاتها شهادات معتمدة تعزز فرصهن في القطاعين الحكومي والخاص.',
            'vision' => 'أن نكون الخيار الأول للتمكين المهني في المملكة عبر تدريب نوعي معتمد.',
            'mission' => 'تقديم برامج تدريبية معتمدة وعالية الجودة، في بيئة محفزة، تؤهل المتدربين لسوق العمل وتواكب رؤية المملكة 2030.',
            'values' => ['الجودة', 'التمكين', 'الالتزام', 'التطوير المستمر'],
        ],
        'stats' => [
            'items' => [
                ['value' => 0, 'label' => 'متدربة', 'suffix' => '+'],
                ['value' => 0, 'label' => 'خريجة', 'suffix' => '+'],
                ['value' => 0, 'label' => 'سنوات خبرة', 'suffix' => '+'],
            ],
        ],
        'contact' => [
            'phone' => '0137247666',
            'email' => 'hsstli7@gmail.com',
            'address' => 'حفر الباطن، المملكة العربية السعودية',
            'hours' => 'الأحد - الخميس، 8 صباحًا - 8 مساءً',
        ],
        'social' => [],
        'seo' => [
            'title' => 'معهد السمو للتدريب | دبلومات ودورات معتمدة للسيدات',
            'description' => 'معهد السمو للتدريب في حفر الباطن: دبلومات ودورات معتمدة من المؤسسة العامة للتدريب التقني والمهني في الإدارة والحاسب واللغة الإنجليزية.',
        ],
    ];
    $existing = array_column(DB::all('SELECT k FROM settings'), 'k');
    foreach ($defaults as $group => $values) {
        if (in_array($group, $existing, true)) continue;
        save_settings_group($group, $values);
        out("settings: $group");
    }

    if (!(int) DB::value('SELECT COUNT(*) FROM pages')) {
        $policies = [
            'سياسة الحضور الإلكتروني', 'سياسات النزاهة الإلكترونية', 'حقوق الملكية الفكرية',
            'سياسة الخصوصية والاستخدام', 'سياسة الاسترداد المالي', 'وحدة متابعة الخريجين', 'آلية الشكاوى والمقترحات',
        ];
        foreach ($policies as $i => $title) {
            DB::insert('pages', [
                'title' => $title,
                'slug' => unique_slug('pages', make_slug($title)),
                'body' => '<p>يتم تحديث محتوى هذه الصفحة من لوحة التحكم.</p>',
                'sort' => $i,
            ]);
        }
        out('pages: ' . count($policies));
    }
    out('seed done');
}

/* ---------------- WordPress import ---------------- */

function import_wp(string $wpConfig, string $uploadsDir): void
{
    if (!is_file($wpConfig)) throw new HttpError(422, "wp-config not found: $wpConfig");
    $src = file_get_contents($wpConfig);
    $get = function (string $k) use ($src): string {
        if (!preg_match("/define\(\s*'$k'\s*,\s*'((?:[^'\\\\]|\\\\.)*)'\s*\)/", $src, $m)) throw new HttpError(422, "missing $k");
        return stripcslashes($m[1]);
    };
    preg_match('/\$table_prefix\s*=\s*\'([^\']+)\'/', $src, $pm);
    $p = $pm[1] ?? 'wp_';
    // WP_DB_PORT lets a local run reach the server's MySQL through an SSH tunnel.
    $port = getenv('WP_DB_PORT') ?: '3306';
    $host = explode(':', $get('DB_HOST'))[0];
    $wp = new PDO("mysql:host=$host;port=$port;dbname={$get('DB_NAME')};charset=utf8mb4", $get('DB_USER'), $get('DB_PASSWORD'), [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION, PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    ]);

    $thumb = function (int $postId) use ($wp, $p, $uploadsDir): ?string {
        $st = $wp->prepare("SELECT m2.meta_value FROM {$p}postmeta m1 JOIN {$p}postmeta m2 ON m2.post_id = m1.meta_value AND m2.meta_key = '_wp_attached_file' WHERE m1.post_id = ? AND m1.meta_key = '_thumbnail_id'");
        $st->execute([$postId]);
        $rel = $st->fetchColumn();
        return $rel ? import_image(rtrim($uploadsDir, '/') . '/' . $rel) : null;
    };
    $meta = function (int $postId, string $key) use ($wp, $p): ?string {
        $st = $wp->prepare("SELECT meta_value FROM {$p}postmeta WHERE post_id = ? AND meta_key = ? LIMIT 1");
        $st->execute([$postId, $key]);
        $v = $st->fetchColumn();
        return $v === false ? null : (string) $v;
    };
    $cats = function (int $postId) use ($wp, $p): string {
        $st = $wp->prepare("SELECT t.name FROM {$p}term_relationships r JOIN {$p}term_taxonomy tt ON tt.term_taxonomy_id = r.term_taxonomy_id JOIN {$p}terms t ON t.term_id = tt.term_id WHERE r.object_id = ? AND tt.taxonomy = 'product_cat'");
        $st->execute([$postId]);
        return implode(' ', $st->fetchAll(PDO::FETCH_COLUMN));
    };

    // Programs (WooCommerce products)
    $n = 0;
    $products = $wp->query("SELECT ID, post_title, post_content, post_excerpt, menu_order FROM {$p}posts WHERE post_type = 'product' AND post_status = 'publish' ORDER BY menu_order, ID")->fetchAll();
    foreach ($products as $i => $prod) {
        $title = trim(html_entity_decode($prod['post_title']));
        if (DB::value('SELECT id FROM programs WHERE title = ?', [$title])) continue;
        $c = $cats((int) $prod['ID']) . ' ' . $title;
        $type = str_contains($c, 'دبلوم') ? 'diploma' : (str_contains($c, 'تطوير') ? 'development' : 'qualifying');
        $price = $meta((int) $prod['ID'], '_price');
        $summary = plain_text(html_entity_decode($prod['post_excerpt']), 600)
            ?? plain_text(mb_substr(html_entity_decode(strip_tags($prod['post_content'])), 0, 300), 600);
        DB::insert('programs', [
            'type' => $type,
            'title' => $title,
            'slug' => unique_slug('programs', make_slug($title)),
            'summary' => $summary,
            'body' => sanitize_html(wpautop_lite($prod['post_content'])),
            'image' => $thumb((int) $prod['ID']),
            'price' => $price && (float) $price > 0 ? rtrim(rtrim($price, '0'), '.') . ' ريال' : null,
            'accreditation' => 'المؤسسة العامة للتدريب التقني والمهني',
            'features' => json_encode([], JSON_UNESCAPED_UNICODE),
            'is_featured' => 1,
            'sort' => $i,
        ]);
        $n++;
    }
    out("programs imported: $n");

    // Blog posts
    $n = 0;
    foreach ($wp->query("SELECT ID, post_title, post_content, post_excerpt, post_date FROM {$p}posts WHERE post_type = 'post' AND post_status = 'publish' ORDER BY post_date DESC")->fetchAll() as $post) {
        $title = trim(html_entity_decode($post['post_title']));
        if ($title === '' || DB::value('SELECT id FROM posts WHERE title = ?', [$title])) continue;
        $body = sanitize_html(wpautop_lite($post['post_content']));
        if ($body === null) continue;
        DB::insert('posts', [
            'title' => $title,
            'slug' => unique_slug('posts', make_slug($title)),
            'excerpt' => plain_text(html_entity_decode($post['post_excerpt']), 600) ?? plain_text(mb_substr(strip_tags($body), 0, 250), 600),
            'body' => $body,
            'cover' => $thumb((int) $post['ID']),
            'published_at' => substr($post['post_date'], 0, 10),
        ]);
        $n++;
    }
    out("posts imported: $n");

    // Policy pages: replace the placeholder body when WordPress has real text
    $n = 0;
    foreach ($wp->query("SELECT post_title, post_content FROM {$p}posts WHERE post_type = 'page' AND post_status = 'publish'")->fetchAll() as $pg) {
        $body = sanitize_html(wpautop_lite($pg['post_content']));
        if ($body === null || mb_strlen(strip_tags($body)) < 40) continue;
        $title = trim(html_entity_decode($pg['post_title']));
        // tolerate common spelling variants ("سياسية" vs "سياسة", hamzas, taa marbuta)
        $norm = fn(string $s) => str_replace(['سياسية', 'ة', 'أ', 'إ', 'ي', ' '], ['سياسه', 'ه', 'ا', 'ا', 'ى', ''], $s);
        foreach (DB::all('SELECT id, title FROM pages') as $mine) {
            if ($norm($mine['title']) === $norm($title) || str_contains($norm($title), $norm($mine['title']))) {
                DB::update('pages', (int) $mine['id'], ['body' => $body]);
                $n++;
                break;
            }
        }
    }
    out("pages updated from WordPress: $n");
}

function import_image(string $path): ?string
{
    if (!is_file($path)) return null;
    try {
        return store_image_file($path, basename($path), null)['url'];
    } catch (Throwable $e) {
        out("  image skipped ($path): {$e->getMessage()}");
        return null;
    }
}

/** WordPress stores paragraphs as blank lines; strip builder shortcodes too. */
function wpautop_lite(string $html): string
{
    $html = preg_replace('/\[\/?[a-z_\-]+[^\]]*\]/i', '', $html) ?? $html;   // [shortcodes]
    $html = preg_replace('/<!--.*?-->/s', '', $html) ?? $html;               // gutenberg comments
    if (!preg_match('/<(p|div|h\d|ul|ol)\b/i', $html)) {
        $parts = preg_split('/\n\s*\n/', trim($html)) ?: [];
        $html = implode('', array_map(fn($x) => '<p>' . nl2br(trim($x)) . '</p>', $parts));
    }
    return $html;
}
