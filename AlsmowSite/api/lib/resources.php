<?php
declare(strict_types=1);

/* ============================================================
   Content types managed from the dashboard.
   One definition drives: DB whitelist, validation, and the
   admin UI (served as JSON from /api/admin/schema).
   Field types: string text html int bool enum date image url list slug
   ============================================================ */

const PROGRAM_TYPES = ['diploma' => 'دبلوم', 'qualifying' => 'دورة تأهيلية', 'development' => 'دورة تطويرية'];
const PROGRAM_MODES = ['onsite' => 'حضوري', 'online' => 'عن بعد', 'hybrid' => 'مدمج'];
/** مجال البرنامج: each one has its own live scene on the site (web/src/site/home/scenes.tsx). */
const PROGRAM_FIELDS = [
    'hr' => 'الموارد البشرية', 'nursing' => 'التمريض', 'health' => 'الإدارة الصحية والمستشفيات', 'ai' => 'الذكاء الاصطناعي والتقنية',
    'law' => 'القانون', 'office' => 'الإدارة المكتبية والسكرتارية', 'computer' => 'الحاسب الآلي وبرامج الأوفيس', 'data' => 'إدخال البيانات',
    'language' => 'اللغات', 'accounting' => 'المحاسبة والمالية', 'marketing' => 'التسويق والمبيعات', 'business' => 'إدارة الأعمال',
];

function resources(): array
{
    $published = ['type' => 'bool', 'label' => 'منشور', 'default' => true, 'column' => true];
    $sort = ['type' => 'int', 'label' => 'الترتيب', 'default' => 0, 'min' => -9999, 'max' => 9999, 'help' => 'الأصغر يظهر أولًا'];

    return [
        'programs' => [
            'label' => 'البرامج', 'singular' => 'برنامج', 'icon' => 'graduation-cap',
            'table' => 'programs', 'title' => 'title', 'order' => 'sort ASC, id DESC',
            'search' => ['title', 'summary'], 'filters' => ['type', 'category'],
            'fields' => [
                'type' => ['type' => 'enum', 'label' => 'نوع البرنامج', 'required' => true, 'options' => PROGRAM_TYPES, 'column' => true],
                'title' => ['type' => 'string', 'label' => 'اسم البرنامج', 'required' => true, 'max' => 200, 'column' => true],
                'category' => ['type' => 'enum', 'label' => 'مجال البرنامج', 'options' => PROGRAM_FIELDS, 'column' => true, 'help' => 'يحدد المشهد الحي الذي يُعرض للبرنامج في الموقع (مثل شاشة متابعة المريض للتمريض)'],
                'careers' => ['type' => 'string', 'label' => 'مجالات العمل بعد التخرج', 'max' => 200, 'placeholder' => 'مثال: أخصائي موارد بشرية · مسؤول توظيف', 'help' => 'تظهر مع البرنامج في الرئيسية؛ اتركها فارغة لاستخدام مجالات المجال الافتراضية'],
                'slug' => ['type' => 'slug', 'label' => 'الرابط المختصر', 'from' => 'title', 'help' => 'يُنشأ تلقائيًا من الاسم إذا تُرك فارغًا'],
                'summary' => ['type' => 'text', 'label' => 'نبذة قصيرة', 'max' => 600, 'help' => 'تظهر على كارت البرنامج'],
                'image' => ['type' => 'image', 'label' => 'صورة البرنامج', 'column' => true],
                'hours' => ['type' => 'int', 'label' => 'عدد الساعات المعتمدة', 'min' => 0, 'max' => 10000, 'column' => true],
                'duration' => ['type' => 'string', 'label' => 'المدة', 'max' => 80, 'placeholder' => 'مثال: 6 أشهر'],
                'mode' => ['type' => 'enum', 'label' => 'طريقة الدراسة', 'options' => PROGRAM_MODES],
                'badge' => ['type' => 'string', 'label' => 'شارة', 'max' => 40, 'placeholder' => 'جديد / الأكثر طلبًا / منتهي بالتوظيف'],
                'accreditation' => ['type' => 'string', 'label' => 'جهة الاعتماد', 'max' => 200, 'placeholder' => 'المؤسسة العامة للتدريب التقني والمهني'],
                'price' => ['type' => 'string', 'label' => 'الرسوم', 'max' => 80, 'placeholder' => 'اتركها فارغة لإخفائها'],
                'features' => ['type' => 'list', 'label' => 'المميزات', 'max' => 200, 'maxItems' => 12, 'help' => 'نقاط قصيرة تظهر على الكارت وصفحة البرنامج'],
                'body' => ['type' => 'html', 'label' => 'التفاصيل الكاملة'],
                'is_featured' => ['type' => 'bool', 'label' => 'يظهر في الرئيسية', 'default' => true, 'column' => true],
                'is_published' => $published,
                'sort' => $sort,
            ],
        ],
        'posts' => [
            'label' => 'المدونة والأخبار', 'singular' => 'مقال', 'icon' => 'newspaper',
            'table' => 'posts', 'title' => 'title', 'order' => 'published_at DESC, id DESC',
            'search' => ['title', 'excerpt'],
            'fields' => [
                'title' => ['type' => 'string', 'label' => 'العنوان', 'required' => true, 'max' => 220, 'column' => true],
                'slug' => ['type' => 'slug', 'label' => 'الرابط المختصر', 'from' => 'title', 'help' => 'يُنشأ تلقائيًا من العنوان إذا تُرك فارغًا'],
                'category' => ['type' => 'string', 'label' => 'التصنيف', 'max' => 80, 'placeholder' => 'أخبار / مقالات / فعاليات', 'column' => true],
                'cover' => ['type' => 'image', 'label' => 'صورة الغلاف', 'column' => true],
                'excerpt' => ['type' => 'text', 'label' => 'مقتطف', 'max' => 600],
                'body' => ['type' => 'html', 'label' => 'المحتوى', 'required' => true],
                'published_at' => ['type' => 'date', 'label' => 'تاريخ النشر', 'default' => 'today', 'column' => true],
                'is_published' => $published,
            ],
        ],
        'pages' => [
            'label' => 'الصفحات والسياسات', 'singular' => 'صفحة', 'icon' => 'file-text',
            'table' => 'pages', 'title' => 'title', 'order' => 'sort ASC, id ASC',
            'search' => ['title'],
            'fields' => [
                'title' => ['type' => 'string', 'label' => 'عنوان الصفحة', 'required' => true, 'max' => 200, 'column' => true],
                'slug' => ['type' => 'slug', 'label' => 'الرابط المختصر', 'from' => 'title'],
                'body' => ['type' => 'html', 'label' => 'المحتوى', 'required' => true],
                'in_menu' => ['type' => 'bool', 'label' => 'تظهر في قائمة الإرشاد الأكاديمي', 'default' => true, 'column' => true],
                'is_published' => $published,
                'sort' => $sort,
            ],
        ],
        'accreditations' => [
            'label' => 'الاعتمادات والشهادات', 'singular' => 'اعتماد', 'icon' => 'badge-check',
            'table' => 'accreditations', 'title' => 'name', 'order' => 'sort ASC, id ASC',
            'search' => ['name'],
            'fields' => [
                'name' => ['type' => 'string', 'label' => 'اسم الجهة / الشهادة', 'required' => true, 'max' => 200, 'column' => true],
                'logo' => ['type' => 'image', 'label' => 'الشعار', 'required' => true, 'column' => true],
                'certificate' => ['type' => 'image', 'label' => 'صورة الشهادة', 'help' => 'تفتح عند الضغط على الشعار'],
                'is_published' => $published,
                'sort' => $sort,
            ],
        ],
        'partners' => [
            'label' => 'الشركاء', 'singular' => 'شريك', 'icon' => 'handshake',
            'table' => 'partners', 'title' => 'name', 'order' => 'sort ASC, id ASC',
            'search' => ['name'],
            'fields' => [
                'name' => ['type' => 'string', 'label' => 'اسم الجهة', 'required' => true, 'max' => 200, 'column' => true],
                'logo' => ['type' => 'image', 'label' => 'الشعار', 'required' => true, 'column' => true],
                'url' => ['type' => 'url', 'label' => 'رابط الموقع', 'max' => 500],
                'is_published' => $published,
                'sort' => $sort,
            ],
        ],
        'agreements' => [
            'label' => 'الاتفاقيات', 'singular' => 'اتفاقية', 'icon' => 'file-signature',
            'table' => 'agreements', 'title' => 'title', 'order' => 'signed_at DESC, id DESC',
            'search' => ['title', 'partner'],
            'fields' => [
                'title' => ['type' => 'string', 'label' => 'عنوان الاتفاقية', 'required' => true, 'max' => 220, 'column' => true],
                'partner' => ['type' => 'string', 'label' => 'الجهة', 'max' => 200, 'column' => true],
                'image' => ['type' => 'image', 'label' => 'صورة', 'column' => true],
                'summary' => ['type' => 'text', 'label' => 'نبذة', 'max' => 600],
                'signed_at' => ['type' => 'date', 'label' => 'تاريخ التوقيع', 'column' => true],
                'is_published' => $published,
            ],
        ],
        'testimonials' => [
            'label' => 'آراء المتدربين', 'singular' => 'رأي', 'icon' => 'message-square-quote',
            'table' => 'testimonials', 'title' => 'name', 'order' => 'sort ASC, id DESC',
            'search' => ['name', 'text'],
            'fields' => [
                'name' => ['type' => 'string', 'label' => 'الاسم', 'required' => true, 'max' => 120, 'column' => true],
                'role' => ['type' => 'string', 'label' => 'البرنامج / الصفة', 'max' => 160, 'placeholder' => 'خريجة دبلوم الموارد البشرية', 'column' => true],
                'text' => ['type' => 'text', 'label' => 'الرأي', 'required' => true, 'max' => 1200],
                'rating' => ['type' => 'int', 'label' => 'التقييم (من 5)', 'default' => 5, 'min' => 1, 'max' => 5, 'column' => true],
                'avatar' => ['type' => 'image', 'label' => 'صورة شخصية'],
                'is_published' => $published,
                'sort' => $sort,
            ],
        ],
    ];
}

function resource(string $name): array
{
    $all = resources();
    if (!isset($all[$name])) throw new HttpError(404, 'نوع المحتوى غير موجود');
    return $all[$name] + ['name' => $name];
}

/** Schema for the admin UI (no table names or server-only bits). */
function admin_schema(): array
{
    $out = [];
    foreach (resources() as $name => $r) {
        $fields = [];
        foreach ($r['fields'] as $key => $f) {
            $field = ['key' => $key] + $f;
            if (isset($f['options'])) {
                $field['options'] = array_map(fn($v, $l) => ['value' => $v, 'label' => $l], array_keys($f['options']), $f['options']);
            }
            if (($f['default'] ?? null) === 'today') $field['default'] = date('Y-m-d');
            $fields[] = $field;
        }
        $out[] = [
            'name' => $name,
            'label' => $r['label'],
            'singular' => $r['singular'],
            'icon' => $r['icon'],
            'title' => $r['title'],
            'filters' => $r['filters'] ?? [],
            'fields' => $fields,
        ];
    }
    return $out;
}

/* ---------------- Validation ---------------- */

/** @return array<string,mixed> clean row ready for DB */
function validate_record(array $res, array $input, ?array $existing): array
{
    $errors = [];
    $row = [];
    foreach ($res['fields'] as $key => $f) {
        $isCreate = $existing === null;
        if (!array_key_exists($key, $input)) {
            if ($isCreate && array_key_exists('default', $f)) {
                $input[$key] = $f['default'] === 'today' ? date('Y-m-d') : $f['default'];
            } elseif ($isCreate && $f['type'] === 'slug') {
                $input[$key] = '';
            } elseif ($isCreate && !empty($f['required'])) {
                $errors[$key] = "{$f['label']} مطلوب";
                continue;
            } else {
                continue; // partial update: leave column untouched
            }
        }
        $v = $input[$key];
        $label = $f['label'];
        $max = $f['max'] ?? null;
        $empty = $v === null || $v === '' || (is_array($v) && !$v);

        if ($empty && !empty($f['required'])) { $errors[$key] = "$label مطلوب"; continue; }

        switch ($f['type']) {
            case 'string':
            case 'text':
                $clean = plain_text($v, $max ?? 255);
                if ($max && $clean !== null && mb_strlen(trim(strip_tags((string) $v))) > $max) {
                    $errors[$key] = "$label يجب ألا يزيد عن $max حرف";
                }
                $row[$key] = $clean;
                break;
            case 'html':
                $row[$key] = sanitize_html(is_string($v) ? $v : null);
                if (!empty($f['required']) && $row[$key] === null) $errors[$key] = "$label مطلوب";
                break;
            case 'int':
                if ($empty) { $row[$key] = null; break; }
                if (!is_numeric($v) || (int) $v != $v) { $errors[$key] = "$label يجب أن يكون رقمًا صحيحًا"; break; }
                $n = (int) $v;
                if (isset($f['min']) && $n < $f['min']) $errors[$key] = "$label يجب ألا يقل عن {$f['min']}";
                if (isset($f['max']) && $n > $f['max']) $errors[$key] = "$label يجب ألا يزيد عن {$f['max']}";
                $row[$key] = $n;
                break;
            case 'bool':
                $row[$key] = filter_var($v, FILTER_VALIDATE_BOOL) ? 1 : 0;
                break;
            case 'enum':
                if ($empty) { $row[$key] = null; break; }
                if (!array_key_exists((string) $v, $f['options'])) { $errors[$key] = "قيمة $label غير صحيحة"; break; }
                $row[$key] = (string) $v;
                break;
            case 'date':
                if ($empty) { $row[$key] = null; break; }
                $d = DateTimeImmutable::createFromFormat('!Y-m-d', (string) $v);
                if (!$d || $d->format('Y-m-d') !== $v) { $errors[$key] = "$label غير صحيح"; break; }
                $row[$key] = $v;
                break;
            case 'image':
                if ($empty) { $row[$key] = null; break; }
                $url = safe_url((string) $v, false);
                if ($url === null) { $errors[$key] = "رابط $label غير صحيح"; break; }
                $row[$key] = mb_substr($url, 0, 500);
                break;
            case 'url':
                if ($empty) { $row[$key] = null; break; }
                $url = safe_url((string) $v);
                if ($url === null) { $errors[$key] = "$label يجب أن يبدأ بـ https://"; break; }
                $row[$key] = mb_substr($url, 0, $max ?? 500);
                break;
            case 'list':
                if ($empty) { $row[$key] = json_encode([]); break; }
                if (!is_array($v)) { $errors[$key] = "$label غير صحيحة"; break; }
                $items = array_values(array_filter(array_map(fn($x) => plain_text($x, $max ?? 200), $v)));
                if (count($items) > ($f['maxItems'] ?? 20)) { $errors[$key] = "الحد الأقصى {$f['maxItems']} عناصر"; break; }
                $row[$key] = json_encode($items, JSON_UNESCAPED_UNICODE);
                break;
            case 'slug':
                $src = trim((string) $v) !== '' ? (string) $v : (string) ($input[$f['from']] ?? $existing[$f['from']] ?? '');
                $row[$key] = unique_slug($res['table'], make_slug($src), $existing ? (int) $existing['id'] : null);
                break;
        }
    }
    if ($errors) throw new HttpError(422, 'يرجى مراجعة الحقول المظللة', $errors);
    return $row;
}

/** Turn DB row into API shape (types restored). */
function shape_row(array $res, array $row): array
{
    $out = ['id' => (int) $row['id']];
    foreach ($res['fields'] as $key => $f) {
        if (!array_key_exists($key, $row)) continue;
        $v = $row[$key];
        $out[$key] = match ($f['type']) {
            'bool' => (bool) $v,
            'int' => $v === null ? null : (int) $v,
            'list' => $v ? (json_decode((string) $v, true) ?: []) : [],
            default => $v,
        };
    }
    foreach (['created_at', 'updated_at'] as $ts) if (isset($row[$ts])) $out[$ts] = $row[$ts];
    return $out;
}
