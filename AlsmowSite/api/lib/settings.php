<?php
declare(strict_types=1);

/* ============================================================
   Site settings, grouped. Stored as one JSON row per group.
   Same field types as resources + "stats" (list of {value,label}).
   ============================================================ */

function settings_schema(): array
{
    return [
        'general' => [
            'label' => 'عام', 'icon' => 'settings',
            'fields' => [
                'site_name' => ['type' => 'string', 'label' => 'اسم المعهد', 'max' => 120],
                'tagline' => ['type' => 'string', 'label' => 'الشعار النصي', 'max' => 200],
                'logo' => ['type' => 'image', 'label' => 'الشعار'],
                'announcement' => ['type' => 'string', 'label' => 'شريط الإعلان أعلى الموقع', 'max' => 200, 'help' => 'اتركه فارغًا لإخفاء الشريط'],
                'announcement_link' => ['type' => 'url', 'label' => 'رابط شريط الإعلان', 'max' => 500],
                'announcement_cta' => ['type' => 'string', 'label' => 'نص زر شريط الإعلان', 'max' => 40, 'placeholder' => 'سجلي الآن', 'help' => 'يظهر بجوار نص الشريط عند وجود رابط'],
                'register_text' => ['type' => 'string', 'label' => 'نص زر التسجيل أعلى الموقع', 'max' => 30, 'placeholder' => 'سجلي الآن'],
                'register_url' => ['type' => 'url', 'label' => 'رابط زر التسجيل', 'max' => 500, 'help' => 'اتركه فارغًا لاستخدام نموذج التسجيل في الموقع'],
                // "hide" (not "show") so an unticked box can never hide the button by accident
                'portal_hide' => ['type' => 'bool', 'label' => 'إخفاء زر بوابة المتدربات أعلى الموقع', 'default' => false],
                'portal_text' => ['type' => 'string', 'label' => 'نص زر بوابة المتدربات', 'max' => 30, 'placeholder' => 'بوابة المتدربات', 'help' => 'الزر بجوار «سجلي الآن» أعلى الموقع (وفي قائمة الجوال)'],
                'portal_link' => ['type' => 'url', 'label' => 'رابط زر بوابة المتدربات', 'max' => 500, 'placeholder' => '/portal', 'help' => 'اتركه فارغًا لفتح بوابة المتدربات في الموقع'],
                'lms_url' => ['type' => 'url', 'label' => 'رابط منصة التعليم الإلكتروني', 'max' => 500],
                'theme_accent' => ['type' => 'enum', 'label' => 'لون الموقع الافتراضي', 'options' => [
                    'semou' => 'السمو', 'emerald' => 'زمردي', 'teal' => 'تركواز', 'sky' => 'سماوي', 'ocean' => 'محيطي', 'navy' => 'كحلي',
                    'indigo' => 'نيلي', 'royal' => 'ملكي', 'lavender' => 'لافندر', 'plum' => 'برقوقي', 'rose' => 'وردي', 'ruby' => 'ياقوتي',
                    'coral' => 'مرجاني', 'sunset' => 'غروب', 'gold' => 'ذهبي', 'olive' => 'زيتوني', 'mocha' => 'موكا', 'graphite' => 'فحمي',
                ], 'help' => 'الزائرة تقدر تغيّره من زر الألوان، وده اللي يظهر لها أول مرة'],
                'theme_mode' => ['type' => 'enum', 'label' => 'المظهر الافتراضي', 'options' => ['auto' => 'تلقائي (حسب الجهاز)', 'light' => 'فاتح', 'dark' => 'داكن']],
            ],
        ],
        'hero' => [
            'label' => 'الواجهة الرئيسية', 'icon' => 'sparkles',
            'fields' => [
                'eyebrow' => ['type' => 'string', 'label' => 'عنوان صغير أعلى العنوان', 'max' => 120],
                'title' => ['type' => 'string', 'label' => 'العنوان الرئيسي', 'max' => 160],
                'highlight' => ['type' => 'string', 'label' => 'كلمة مميزة باللون', 'max' => 80, 'help' => 'جزء من العنوان يظهر بلون مختلف'],
                'subtitle' => ['type' => 'text', 'label' => 'الوصف', 'max' => 500],
                'cta_text' => ['type' => 'string', 'label' => 'نص الزر الأساسي', 'max' => 60],
                'cta_link' => ['type' => 'url', 'label' => 'رابط الزر الأساسي', 'max' => 500],
                'secondary_text' => ['type' => 'string', 'label' => 'نص الزر الثانوي', 'max' => 60],
                'secondary_link' => ['type' => 'url', 'label' => 'رابط الزر الثانوي', 'max' => 500],
                'image' => ['type' => 'image', 'label' => 'صورة الواجهة'],
                'highlights' => ['type' => 'list', 'label' => 'نقاط مميزة', 'max' => 80, 'maxItems' => 4],
            ],
        ],
        // The opening scene: doves carrying the welcome banner (web/src/site/IntroBirds.tsx)
        'intro' => [
            'label' => 'الافتتاحية', 'icon' => 'bird',
            'fields' => [
                'mode' => ['type' => 'enum', 'label' => 'متى تظهر', 'default' => 'session', 'options' => [
                    'session' => 'مرة في كل زيارة',
                    'first' => 'أول زيارة فقط لكل جهاز',
                    'always' => 'دائمًا مع كل فتح للموقع',
                    'off' => 'متوقفة',
                ], 'help' => 'للمعاينة في أي وقت: افتح الموقع ومعه ‎?intro‎ في آخر الرابط'],
                'kicker' => ['type' => 'string', 'label' => 'السطر الصغير أعلى اللافتة', 'max' => 60, 'placeholder' => 'أهلاً بكِ في'],
                'title' => ['type' => 'string', 'label' => 'العنوان الكبير', 'max' => 80, 'placeholder' => 'اسم المعهد', 'help' => 'اتركه فارغًا لعرض اسم المعهد من «عام»'],
                'message' => ['type' => 'string', 'label' => 'الرسالة', 'max' => 120, 'placeholder' => 'الشعار النصي', 'help' => 'اتركه فارغًا لعرض الشعار النصي من «عام»'],
                'quote' => ['type' => 'string', 'label' => 'الحكمة أسفل اللافتة', 'max' => 120, 'placeholder' => '﴿وَقُل رَّبِّ زِدْنِي عِلْمًا﴾', 'help' => 'اتركها فارغة لإخفائها'],
                'skip' => ['type' => 'string', 'label' => 'نص زر التخطي', 'max' => 20, 'placeholder' => 'تخطّي'],
            ],
        ],
        'about' => [
            'label' => 'عن المعهد', 'icon' => 'building-2',
            'fields' => [
                'title' => ['type' => 'string', 'label' => 'العنوان', 'max' => 160],
                'summary' => ['type' => 'text', 'label' => 'نبذة (للرئيسية)', 'max' => 700],
                'body' => ['type' => 'html', 'label' => 'المحتوى الكامل (صفحة عن المعهد)'],
                'image' => ['type' => 'image', 'label' => 'صورة'],
                'vision' => ['type' => 'text', 'label' => 'الرؤية', 'max' => 500],
                'mission' => ['type' => 'text', 'label' => 'الرسالة', 'max' => 500],
                'values' => ['type' => 'list', 'label' => 'القيم', 'max' => 80, 'maxItems' => 8],
            ],
        ],
        'stats' => [
            'label' => 'الإحصائيات', 'icon' => 'bar-chart-3',
            'fields' => [
                'items' => ['type' => 'stats', 'label' => 'العدادات', 'maxItems' => 6, 'help' => 'أرقام تظهر بعداد متحرك في الرئيسية'],
            ],
        ],
        'contact' => [
            'label' => 'التواصل', 'icon' => 'phone',
            'fields' => [
                'phone' => ['type' => 'string', 'label' => 'الهاتف', 'max' => 30],
                'whatsapp' => ['type' => 'string', 'label' => 'واتساب (بالصيغة الدولية)', 'max' => 30, 'placeholder' => '9665XXXXXXXX'],
                'email' => ['type' => 'string', 'label' => 'البريد الإلكتروني', 'max' => 190],
                'address' => ['type' => 'string', 'label' => 'العنوان', 'max' => 250],
                'hours' => ['type' => 'string', 'label' => 'أوقات العمل', 'max' => 120],
                'map_url' => ['type' => 'url', 'label' => 'رابط خرائط جوجل', 'max' => 1000, 'help' => 'من خرائط جوجل: مشاركة ← تضمين خريطة ← انسخ الرابط من src'],
                'notify_email' => ['type' => 'string', 'label' => 'بريد استلام إشعارات الرسائل', 'max' => 190, 'help' => 'يصلك بريد عند كل رسالة أو طلب تسجيل جديد'],
            ],
        ],
        'social' => [
            'label' => 'التواصل الاجتماعي', 'icon' => 'share-2',
            'fields' => [
                'x' => ['type' => 'url', 'label' => 'X (تويتر)', 'max' => 300],
                'instagram' => ['type' => 'url', 'label' => 'إنستجرام', 'max' => 300],
                'snapchat' => ['type' => 'url', 'label' => 'سناب شات', 'max' => 300],
                'tiktok' => ['type' => 'url', 'label' => 'تيك توك', 'max' => 300],
                'facebook' => ['type' => 'url', 'label' => 'فيسبوك', 'max' => 300],
                'linkedin' => ['type' => 'url', 'label' => 'لينكدإن', 'max' => 300],
                'youtube' => ['type' => 'url', 'label' => 'يوتيوب', 'max' => 300],
            ],
        ],
        'seo' => [
            'label' => 'محركات البحث', 'icon' => 'search',
            'fields' => [
                'title' => ['type' => 'string', 'label' => 'عنوان الموقع في جوجل', 'max' => 70],
                'description' => ['type' => 'text', 'label' => 'الوصف في جوجل', 'max' => 170],
                'og_image' => ['type' => 'image', 'label' => 'صورة المشاركة (واتساب وتويتر)'],
            ],
        ],
        // «تحدث إلى السمو»: the institute's own assistant (api/lib/assistant.php), no external AI.
        'assistant' => [
            'label' => 'المساعد الذكي', 'icon' => 'bot',
            'fields' => [
                // "off" (not "on") so a never-saved / unticked box can't switch the chat off by accident
                'disabled' => ['type' => 'bool', 'label' => 'إيقاف المساعد في الموقع', 'default' => false],
                'name' => ['type' => 'string', 'label' => 'اسم المساعد', 'max' => 40, 'placeholder' => 'السمو'],
                'title' => ['type' => 'string', 'label' => 'عنوان نافذة المحادثة', 'max' => 60, 'placeholder' => 'تحدث إلى السمو'],
                'welcome' => ['type' => 'text', 'label' => 'رسالة الترحيب', 'max' => 400, 'placeholder' => 'أهلًا بكِ، أنا السمو، مساعدتك في المعهد. اسأليني عن البرامج والتسجيل والرسوم والمواعيد والسياسات.'],
                'suggestions' => ['type' => 'list', 'label' => 'أسئلة مقترحة في البداية', 'max' => 80, 'maxItems' => 6],
                'fallback' => ['type' => 'text', 'label' => 'الرد عند عدم وجود إجابة', 'max' => 300, 'placeholder' => 'لم أجد إجابة مؤكدة لسؤالك في معلومات المعهد.', 'help' => 'يُضاف بعده تلقائيًا رقم الهاتف وواتساب وصفحة التواصل'],
                'kb' => ['type' => 'kb', 'label' => 'قاعدة المعرفة (أسئلة وأجوبة المعهد)', 'maxItems' => 500, 'help' => 'المساعد يعرف تلقائيًا البرامج والسياسات والتواصل من محتوى الموقع؛ أضيفي هنا أي معلومة أخرى: الرسوم، الشروط، المواعيد، الخصومات...'],
            ],
        ],
        // Every text of the site can be changed / hidden / added to without touching the code.
        // Edited by clicking the text on the site («وضع التعديل») or from this tab in the dashboard.
        'texts' => [
            'label' => 'نصوص الموقع', 'icon' => 'type',
            'fields' => [
                'rules' => ['type' => 'textrules', 'label' => 'النصوص المعدّلة', 'maxItems' => 3000],
            ],
        ],
    ];
}

function settings_admin_schema(): array
{
    $out = [];
    foreach (settings_schema() as $group => $g) {
        $fields = [];
        foreach ($g['fields'] as $key => $f) {
            $field = ['key' => $key] + $f;
            if (isset($f['options'])) {
                $field['options'] = array_map(fn($v, $l) => ['value' => $v, 'label' => $l], array_keys($f['options']), $f['options']);
            }
            $fields[] = $field;
        }
        $out[] = ['group' => $group, 'label' => $g['label'], 'icon' => $g['icon'], 'fields' => $fields];
    }
    return $out;
}

function all_settings(): array
{
    $out = array_fill_keys(array_keys(settings_schema()), []);
    foreach (DB::all('SELECT k, v FROM settings') as $r) {
        if (array_key_exists($r['k'], $out)) $out[$r['k']] = json_decode($r['v'], true) ?: [];
    }
    // contact.notify_email is private: never part of the public payload
    return $out;
}

function public_settings(): array
{
    $s = all_settings();
    unset($s['contact']['notify_email']);
    return $s;
}

function save_settings_group(string $group, array $input): array
{
    $schema = settings_schema();
    if (!isset($schema[$group])) throw new HttpError(404, 'مجموعة الإعدادات غير موجودة');

    $errors = [];
    $clean = [];
    foreach ($schema[$group]['fields'] as $key => $f) {
        $v = $input[$key] ?? null;
        $empty = $v === null || $v === '' || (is_array($v) && !$v);
        switch ($f['type']) {
            case 'string':
            case 'text':
                $clean[$key] = plain_text($v, $f['max'] ?? 255);
                break;
            case 'html':
                $clean[$key] = sanitize_html(is_string($v) ? $v : null);
                break;
            case 'bool':
                $clean[$key] = filter_var($v, FILTER_VALIDATE_BOOL);
                break;
            case 'enum':
                $clean[$key] = !$empty && array_key_exists((string) $v, $f['options']) ? (string) $v : null;
                break;
            case 'image':
                $clean[$key] = $empty ? null : safe_url((string) $v, false);
                if (!$empty && $clean[$key] === null) $errors[$key] = "رابط {$f['label']} غير صحيح";
                break;
            case 'url':
                $clean[$key] = $empty ? null : safe_url((string) $v);
                if (!$empty && $clean[$key] === null) $errors[$key] = "{$f['label']}: الرابط يجب أن يبدأ بـ https://";
                break;
            case 'list':
                $items = is_array($v) ? array_values(array_filter(array_map(fn($x) => plain_text($x, $f['max'] ?? 200), $v))) : [];
                $clean[$key] = array_slice($items, 0, $f['maxItems'] ?? 20);
                break;
            case 'stats':
                $items = [];
                foreach (is_array($v) ? $v : [] as $item) {
                    $label = plain_text($item['label'] ?? null, 60);
                    $value = (int) ($item['value'] ?? 0);
                    if ($label === null) continue;
                    $items[] = ['value' => max(0, $value), 'label' => $label, 'suffix' => plain_text($item['suffix'] ?? '+', 4) ?? ''];
                }
                $clean[$key] = array_slice($items, 0, $f['maxItems'] ?? 6);
                break;
            case 'kb':
                $items = [];
                foreach (is_array($v) ? $v : [] as $item) {
                    if (!is_array($item)) continue;
                    $q = plain_text($item['q'] ?? null, 300);
                    $a = plain_text($item['a'] ?? null, 3000);
                    if ($q === null || $a === null) continue;
                    $variants = array_values(array_filter(array_map(fn($x) => plain_text($x, 200), is_array($item['variants'] ?? null) ? $item['variants'] : [])));
                    $link = plain_text($item['link'] ?? null, 500);
                    if ($link !== null && !str_starts_with($link, '/') && safe_url($link) === null) $link = null;
                    $items[] = ['q' => $q, 'variants' => array_slice($variants, 0, 20), 'a' => $a, 'link' => $link];
                }
                $clean[$key] = array_slice($items, 0, $f['maxItems'] ?? 500);
                break;
            case 'textrules':
                $items = [];
                $seen = [];
                foreach (is_array($v) ? $v : [] as $item) {
                    if (!is_array($item)) continue;
                    $from = plain_text($item['from'] ?? null, 600);
                    if ($from === null) continue;
                    $add = in_array($item['add'] ?? null, ['before', 'after'], true) ? $item['add'] : null;
                    $remove = !$add && filter_var($item['remove'] ?? false, FILTER_VALIDATE_BOOL);
                    $to = plain_text($item['to'] ?? null, 1000) ?? '';
                    if (!$remove && $to === '') continue;          // nothing to do
                    if (!$add && !$remove && $to === $from) continue; // same as the original
                    $rule = ['from' => $from, 'to' => $remove ? '' : $to, 'mode' => ($item['mode'] ?? '') === 'contains' && !$add ? 'contains' : 'exact'];
                    if ($remove) {
                        $rule['remove'] = true;
                        if (filter_var($item['whole'] ?? false, FILTER_VALIDATE_BOOL)) { $rule['whole'] = true; $rule['mode'] = 'exact'; }
                    }
                    if ($add) {
                        $rule['add'] = $add;
                        $rule['style'] = in_array($item['style'] ?? null, ['text', 'title', 'note', 'warn'], true) ? $item['style'] : 'text';
                        $path = plain_text($item['path'] ?? null, 300);
                        if ($path !== null && str_starts_with($path, '/')) $rule['path'] = $path;
                    }
                    $k = ($add ?? 'edit') . '|' . ($add ? ($rule['path'] ?? '') : '') . '|' . $from;
                    $seen[$k] = $rule; // the last one wins, like the editor
                }
                $clean[$key] = array_slice(array_values($seen), 0, $f['maxItems'] ?? 3000);
                break;
        }
    }
    if ($group === 'contact') {
        foreach (['email', 'notify_email'] as $k) {
            if (!empty($clean[$k]) && !filter_var($clean[$k], FILTER_VALIDATE_EMAIL)) $errors[$k] = 'البريد الإلكتروني غير صحيح';
        }
        if (!empty($clean['whatsapp'])) $clean['whatsapp'] = preg_replace('/\D+/', '', $clean['whatsapp']);
        if (!empty($clean['map_url']) && !str_starts_with($clean['map_url'], 'https://www.google.com/maps')) {
            $errors['map_url'] = 'يجب أن يكون رابط تضمين من خرائط جوجل (يبدأ بـ https://www.google.com/maps)';
        }
    }
    if ($errors) throw new HttpError(422, 'يرجى مراجعة الحقول المظللة', $errors);

    DB::run(
        'INSERT INTO settings (k, v) VALUES (?, ?) ON DUPLICATE KEY UPDATE v = VALUES(v)',
        [$group, json_encode($clean, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES)]
    );
    return $clean;
}
