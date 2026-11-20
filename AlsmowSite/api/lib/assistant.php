<?php
declare(strict_types=1);

/* ============================================================
   «تحدث إلى السمو» — the institute's own assistant, no external AI.

   How it answers:
   1. understands the question: Arabic normalisation (أ/إ/آ→ا، ة→ه، ى→ي، no harakat), light stemming,
      and a synonym map that folds dialect into one meaning («كام / بكم / سعر» → fees, «فين / وين» → place…);
   2. recognises intents (greeting, thanks, list programs, how to register, contact, portal…);
   3. finds the program asked about (also from the previous turn: «وكم مدتها؟»);
   4. otherwise ranks a knowledge base with BM25: the dashboard's Q&A (settings › المساعد الذكي) plus
      everything already on the site (programs, policy pages, about, contact, posts);
   5. answers from that data only; when unsure it says so and offers related questions + contact.
   ============================================================ */

require_once __DIR__ . '/assistant_core.php';

const AS_MAX_Q = 400;

/* ---------- language ---------- */

function as_norm(string $s): string
{
    $s = preg_replace('/[\x{064B}-\x{065F}\x{0670}\x{0640}]/u', '', $s) ?? $s;
    $s = strtr($s, ['أ' => 'ا', 'إ' => 'ا', 'آ' => 'ا', 'ٱ' => 'ا', 'ة' => 'ه', 'ى' => 'ي', 'ؤ' => 'و', 'ئ' => 'ي', 'گ' => 'ك', 'ڤ' => 'ف', 'پ' => 'ب', 'چ' => 'ج']);
    $s = mb_strtolower($s, 'UTF-8');
    $s = strtr($s, ['٠' => '0', '١' => '1', '٢' => '2', '٣' => '3', '٤' => '4', '٥' => '5', '٦' => '6', '٧' => '7', '٨' => '8', '٩' => '9']);
    $s = preg_replace('/[^\p{L}\p{N}\s]+/u', ' ', $s) ?? $s;
    return trim(preg_replace('/\s+/u', ' ', $s) ?? $s);
}

/** Words that carry no meaning for search. */
function as_stop(): array
{
    static $s = null;
    return $s ??= array_flip(explode(' ', as_norm(
        'في من على الى إلى عن هل ما ماذا هو هي هم انا أنا انت أنت انتي ايه إيه اي أي يا لو مع او أو ثم كل هذا هذه ذلك تلك عندكم عندك عند يوجد فيه فيها كان تكون '
        . 'لي لنا ليا ممكن اريد أريد عايز عاوزه عايزه ابغى ابي ودي حاب حابه بدي محتاج محتاجه اعرف أعرف معرفه ممكن لو سمحت سمحتي من فضلك فضلك الله '
        . 'بس طيب تمام يعني اللي الي التي الذي الذين و بالنسبه نفس جدا كده كذا شو شنو وش ايش إيش هاد هذي دي ده انه أنه اذا إذا متاح متاحه '
        . 'مين قد اقدر أقدر يقدر نقدر ينفع ممكن اعمل أعمل ازاي إزاي كيف كيفيه طريقه حاجه شي شيء موجود موجوده '
        // friendly fillers people add to any question
        . 'يا ي صاحبي صاحبتي صديقي صديقتي قلبي حبيبي حبيبتي معلم يامعلم عالمي غالي غاليه عزيزي عزيزتي اخوي اختي يلا بقا بقى طب '
        . 'معلومات معلومه تفاصيل داخل بخصوص حول اسال أسأل سؤال استفسار استفسارات'
    )));
}

/** Synonym groups: every variant (normalised) maps to a canonical token. */
function as_syn(): array
{
    static $map = null;
    if ($map !== null) return $map;
    $groups = [
        '#fees' => 'رسوم رسم سعر سعره سعرها اسعار أسعار تكلفه تكاليف فلوس مبلغ قيمه مصاريف مصروفات',
        // "how much / how many": price only when nothing else is asked (see as_answer)
        '#howmuch' => 'كم بكم كام بكام قديش',
        '#register' => 'تسجيل سجل اسجل أسجل التسجيل التحاق التحق الحق انضم انضمام اشترك اشتراك قبول التقديم تقديم اقدم أقدم قدم',
        '#place' => 'عنوان مكان موقع فين وين اين أين لوكيشن خريطه مقر مقركم الفرع فرع',
        '#hours' => 'دوام مواعيد موعد متى متي امتى امتي يفتح تفتحون تفتحوا الدوام',
        '#contact' => 'تواصل اتصال اتصل رقم هاتف جوال تليفون تلفون موبايل واتس واتساب وتساب ايميل إيميل بريد كلمكم اكلمكم أكلمكم',
        '#duration' => 'مده مدة مدته مدتها مدتو طول طوله شهر شهور اشهر أشهر سنه سنوات يستغرق تستغرق يطول',
        '#credit' => 'ساعه ساعات',
        '#mode' => 'اونلاين أونلاين حضوري انتظام مدمج',
        '#cert' => 'شهاده شهادات اعتماد معتمد مؤسسه المؤسسه التقني والمهني معترف',
        '#diploma' => 'دبلوم دبلومات دبلومه',
        '#course' => 'دوره دورات كورس كورسات',
        '#program' => 'برنامج برامج تخصص تخصصات اقسام أقسام مجالات',
        '#portal' => 'بوابه بوابتي حسابي حساب مدفوعاتي نتايج نتائج درجات جدول دخول',
        '#lms' => 'منصه المنصه التعليم الالكتروني الإلكتروني',
        '#women' => 'نساء نسائي نسائيه بنات طالبات للبنات رجال شباب للرجال',
        '#hello' => 'سلام السلام مرحبا مرحبتين اهلا أهلا هلا هاي هلو صباح مساء',
        '#thanks' => 'شكرا شكراً مشكور مشكوره تسلم تسلمي يعطيك جزاك ممنون',
        '#refund' => 'استرداد استرجاع الغاء إلغاء انسحاب انسحب ارجاع',
        '#attend' => 'حضور غياب غيابات اغيب أغيب غبت يغيب تغيب تاخير تأخير اتاخر أتأخر',
        '#complain' => 'شكوي شكاوي شكوى شكاوى مقترح مقترحات اقتراح',
        '#privacy' => 'خصوصيه بياناتي',
        '#jobs' => 'توظيف وظيفه وظايف وظائف عمل خريجين خريجات سوق',
        '#about' => 'المعهد معهد رؤيه رؤية رساله قيم نبذه عنكم',
        '#advice' => 'يناسبني يناسب يناسبنى تناسبني انسب الانسب انصحيني انصحني تنصحيني تنصحني رشحي رشحيلي رشحلي اقترحي محتاره محتار حايره حاير اختار',
        '#reqs' =>'شروط شرط متطلبات مطلوب اوراق أوراق مستندات وثائق عمر سن مؤهل',
    ];
    $map = [];
    foreach ($groups as $canon => $words) {
        foreach (explode(' ', as_norm($words)) as $w) {
            if ($w === '') continue;
            $map[$w] = $canon;
            // the same word with "ال" (helps typo matching: «الرسوو» → «الرسوم»)
            if (!str_starts_with($w, 'ال') && mb_strlen($w) >= 3) $map['ال' . $w] ??= $canon;
        }
    }
    return $map;
}

/** Light Arabic stemming: common prefixes / suffixes, keeping at least 3 letters. */
function as_stem(string $w): string
{
    if (mb_strlen($w) <= 3) return $w;
    foreach (['وبال', 'وال', 'بال', 'فال', 'كال', 'لل', 'ال'] as $p) {
        if (str_starts_with($w, $p) && mb_strlen($w) - mb_strlen($p) >= 3) { $w = mb_substr($w, mb_strlen($p)); break; }
    }
    if (mb_strlen($w) > 4 && str_starts_with($w, 'و')) $w = mb_substr($w, 1);
    foreach (['ات', 'ون', 'ين', 'ها', 'هم', 'كم', 'نا', 'يه', 'ه', 'ي'] as $s) {
        if (str_ends_with($w, $s) && mb_strlen($w) - mb_strlen($s) >= 3) { $w = mb_substr($w, 0, -mb_strlen($s)); break; }
    }
    return $w;
}

/** Text → meaningful tokens (synonyms folded to their canonical #tag, the rest stemmed). */
function as_tokens(string $text, bool $keepStop = false, bool $fuzzy = false): array
{
    $syn = as_syn();
    $stop = as_stop();
    $out = [];
    $words = explode(' ', as_norm($text));
    for ($i = 0, $n = count($words); $i < $n; $i++) {
        $w = $words[$i];
        if ($w === '') continue;
        // two-word phrases first ("عن بعد", "اون لاين")
        if ($i + 1 < $n) {
            $pair = $w . ' ' . $words[$i + 1];
            if ($pair === 'عن بعد' || $pair === 'اون لاين') { $out[] = '#mode'; $i++; continue; }
        }
        // leading "و" (and): «وكام» «ومدته» «والرسوم»
        if (mb_strlen($w) > 2 && str_starts_with($w, 'و') && !isset($syn[$w])) {
            $r = mb_substr($w, 1);
            $rb = preg_replace('/ت(ه|ها|هم|و|ك|كم)$/u', 'ه', $r);
            if (isset($syn[$r]) || isset($stop[$r]) || isset($syn[as_stem($r)]) || isset($syn[$rb])) $w = $r;
        }
        if (isset($syn[$w])) { $out[] = $syn[$w]; continue; }
        $st = as_stem($w);
        if (isset($syn[$st])) { $out[] = $syn[$st]; continue; }
        // possessive "ـته / ـتها" (مدته، رسومته…) → the base word with ة
        $base = preg_replace('/ت(ه|ها|هم|و|ك|كم)$/u', 'ه', $w);
        if ($base !== $w && isset($syn[$base])) { $out[] = $syn[$base]; continue; }
        if (!$keepStop && (isset($stop[$w]) || mb_strlen($w) < 2)) continue;
        // a typo of a known word («الرسوم» written «الرسوو»): fold it to that word's meaning
        // (questions only: documents are spelled correctly and fuzzy matching every word would be slow)
        if ($fuzzy) {
            $near = as_fuzzy($w, $syn) ?? as_fuzzy($st, $syn);
            if ($near !== null) { $out[] = $syn[$near]; continue; }
        }
        $out[] = $st;
    }
    return $out;
}

/* ---------- knowledge ---------- */

function as_site(): string { return rtrim((string) config('site_url'), '/'); }

function as_plain(?string $html, int $max = 0): string
{
    if (!$html) return '';
    $t = preg_replace('#<(br|/p|/h\d|/li)[^>]*>#i', "\n", $html) ?? $html;
    $t = html_entity_decode(strip_tags($t), ENT_QUOTES | ENT_HTML5, 'UTF-8');
    $t = trim(preg_replace("/[ \t]+/u", ' ', preg_replace("/\n{2,}/", "\n", $t) ?? $t) ?? $t);
    return $max && mb_strlen($t) > $max ? rtrim(mb_substr($t, 0, $max)) . '…' : $t;
}

/** Settings of the assistant, with defaults for anything never saved. */
function as_settings(): array
{
    $a = all_settings()['assistant'] ?? [];
    $pick = fn(string $k, string $def) => (isset($a[$k]) && is_string($a[$k]) && trim($a[$k]) !== '') ? trim($a[$k]) : $def;
    return [
        'enabled' => ($a['disabled'] ?? false) !== true,
        'name' => $pick('name', 'السمو'),
        'fallback' => $pick('fallback', 'لم أجد إجابة مؤكدة لسؤالك في معلومات المعهد.'),
        'kb' => is_array($a['kb'] ?? null) ? $a['kb'] : [],
    ];
}

/**
 * Every document the assistant can answer from.
 * Each: [id, kind, title, fields => [[text, weight]...], answer (markdown), link?, program?]
 */
function as_docs(?array $kbOverride = null): array
{
    $site = as_site();
    $s = all_settings();
    $g = $s['general'] ?? [];
    $c = $s['contact'] ?? [];
    $ab = $s['about'] ?? [];
    $name = $g['site_name'] ?? 'معهد السمو للتدريب';
    $docs = [];

    // 1) the dashboard's own Q&A (highest trust)
    foreach ($kbOverride ?? as_settings()['kb'] as $i => $e) {
        $q = trim((string) ($e['q'] ?? ''));
        $a = trim((string) ($e['a'] ?? ''));
        if ($q === '' || $a === '') continue;
        $variants = is_array($e['variants'] ?? null) ? implode(' ', $e['variants']) : '';
        $link = trim((string) ($e['link'] ?? ''));
        $docs[] = [
            'id' => "kb$i", 'kind' => 'kb', 'title' => $q, 'boost' => 1.35,
            'fields' => [[$q, 3.0], [$variants, 3.0], [$a, 0.8]],
            'answer' => $a . ($link !== '' ? "\n" . (str_starts_with($link, '/') ? $site . $link : $link) : ''),
        ];
    }

    // 1b) built-in core FAQ, answered from the institute's real data (assistant_core.php)
    foreach (as_core_docs() as $d) $docs[] = $d;

    // 2) programs
    foreach (as_programs() as $p) {
        $docs[] = [
            'id' => 'p' . $p['id'], 'kind' => 'program', 'title' => $p['title'], 'program' => $p, 'boost' => 1.0,
            'fields' => [[$p['title'], 4.0], [PROGRAM_TYPES[$p['type']] ?? '', 1.0], [(string) $p['summary'], 1.0], [implode(' ', $p['features']), 0.8], [as_plain($p['body'], 1500), 0.4]],
            'answer' => as_program_answer($p, []),
        ];
    }

    // 3) policy / guidance pages, in chunks
    foreach (DB::all('SELECT title, slug, body FROM pages WHERE is_published = 1 ORDER BY sort, id') as $pg) {
        $text = as_plain((string) $pg['body']);
        $link = "$site/p/" . rawurlencode($pg['slug']);
        $chunks = as_chunks($text, 520);
        foreach ($chunks as $k => $ch) {
            $docs[] = [
                'id' => "pg{$pg['slug']}-$k", 'kind' => 'page', 'title' => $pg['title'], 'boost' => 0.9,
                'fields' => [[$pg['title'], 3.0], [$ch, 1.0]],
                'answer' => "**{$pg['title']}**\n" . as_plain($ch, 520) . "\nالتفاصيل كاملة: $link",
            ];
        }
    }

    // 4) about
    $about = array_filter([
        $ab['summary'] ?? null,
        !empty($ab['vision']) ? "**الرؤية:** {$ab['vision']}" : null,
        !empty($ab['mission']) ? "**الرسالة:** {$ab['mission']}" : null,
        !empty($ab['values']) ? '**القيم:** ' . implode('، ', (array) $ab['values']) : null,
    ]);
    if ($about) {
        $docs[] = [
            'id' => 'about', 'kind' => 'about', 'title' => "عن $name", 'boost' => 1.0,
            'fields' => [["عن المعهد نبذه رؤيه رساله قيم $name", 2.5], [implode(' ', $about), 1.0]],
            'answer' => "**$name**\n" . implode("\n", $about) . "\nالمزيد: $site/about",
        ];
    }

    // 5) posts
    foreach (DB::all('SELECT title, slug, excerpt FROM posts WHERE is_published = 1 ORDER BY published_at DESC LIMIT 20') as $po) {
        $docs[] = [
            'id' => 'post' . $po['slug'], 'kind' => 'post', 'title' => $po['title'], 'boost' => 0.7,
            'fields' => [[$po['title'], 2.5], [(string) $po['excerpt'], 1.0]],
            'answer' => "**{$po['title']}**\n" . as_plain((string) $po['excerpt'], 400) . "\nاقرئي المقال: $site/blog/" . rawurlencode($po['slug']),
        ];
    }
    return $docs;
}

function as_chunks(string $text, int $size): array
{
    $paras = preg_split("/\n+/u", $text) ?: [];
    $out = [];
    $cur = '';
    foreach ($paras as $p) {
        $p = trim($p);
        if ($p === '') continue;
        if ($cur !== '' && mb_strlen($cur) + mb_strlen($p) > $size) { $out[] = $cur; $cur = ''; }
        $cur .= ($cur === '' ? '' : "\n") . $p;
    }
    if ($cur !== '') $out[] = $cur;
    return $out ?: [$text];
}

function as_programs(): array
{
    static $rows = null;
    if ($rows !== null) return $rows;
    $rows = [];
    foreach (DB::all('SELECT * FROM programs WHERE is_published = 1 ORDER BY type, sort, id') as $p) {
        $p['features'] = json_decode((string) $p['features'], true) ?: [];
        $rows[] = $p;
    }
    return $rows;
}

/* ---------- answers ---------- */

function as_contact_lines(bool $short = false): array
{
    $c = all_settings()['contact'] ?? [];
    $site = as_site();
    $l = [];
    if (!empty($c['phone'])) $l[] = "- **الهاتف:** {$c['phone']}";
    if (!empty($c['whatsapp'])) $l[] = "- **واتساب:** https://wa.me/" . preg_replace('/\D+/', '', (string) $c['whatsapp']);
    if (!$short && !empty($c['email'])) $l[] = "- **البريد:** {$c['email']}";
    if (!$short && !empty($c['address'])) $l[] = "- **العنوان:** {$c['address']}";
    if (!$short && !empty($c['hours'])) $l[] = "- **أوقات العمل:** {$c['hours']}";
    $l[] = "- صفحة التواصل: $site/contact";
    return $l;
}

/** A program card; $focus = the asked-about attributes (#fees, #duration, #mode, #cert, #register…). */
function as_program_answer(array $p, array $focus): string
{
    $site = as_site();
    $type = PROGRAM_TYPES[$p['type']] ?? '';
    $link = "$site/programs/" . rawurlencode((string) $p['slug']);
    $lines = ["**{$p['title']}**" . ($type ? " ({$type})" : '')];
    $want = fn(string $t) => in_array($t, $focus, true);
    $any = false;

    if ($want('#fees')) {
        $any = true;
        $lines[] = $p['price'] ? "- **الرسوم:** {$p['price']}" : '- **الرسوم:** غير معلنة على الموقع، ويسعد فريق القبول بإفادتك بها وبخيارات السداد المتاحة.';
    }
    if ($want('#duration') || $want('#credit')) {
        $any = true;
        if ($p['duration']) $lines[] = "- **المدة:** {$p['duration']}";
        if ($p['hours']) $lines[] = "- **الساعات المعتمدة:** {$p['hours']} ساعة";
        if (!$p['duration'] && !$p['hours']) $lines[] = '- **المدة:** غير مذكورة على الموقع، تواصل معنا لمعرفة الجدول.';
    }
    if ($want('#mode')) {
        $any = true;
        $lines[] = $p['mode'] ? '- **طريقة الدراسة:** ' . (PROGRAM_MODES[$p['mode']] ?? $p['mode']) : '- **طريقة الدراسة:** غير محددة على الموقع، تواصل معنا للتأكد.';
    }
    if ($want('#cert')) {
        $any = true;
        $lines[] = $p['accreditation'] ? "- **الاعتماد:** {$p['accreditation']}" : '- **الاعتماد:** غير مذكور على صفحة البرنامج.';
    }
    if (!$any) {
        if ($p['summary']) $lines[] = trim((string) $p['summary']);
        $facts = [];
        if ($p['duration']) $facts[] = "- **المدة:** {$p['duration']}";
        if ($p['hours']) $facts[] = "- **الساعات المعتمدة:** {$p['hours']} ساعة";
        if ($p['mode']) $facts[] = '- **طريقة الدراسة:** ' . (PROGRAM_MODES[$p['mode']] ?? $p['mode']);
        if ($p['accreditation']) $facts[] = "- **الاعتماد:** {$p['accreditation']}";
        if ($p['price']) $facts[] = "- **الرسوم:** {$p['price']}";
        $lines = array_merge($lines, $facts);
        foreach (array_slice($p['features'], 0, 4) as $f) $lines[] = "- $f";
    }
    $lines[] = "صفحة البرنامج: $link";
    if ($want('#register') || $want('#fees')) $lines[] = "للتسجيل: $site/register";
    return implode("\n", $lines);
}

function as_programs_list(?string $type): string
{
    $site = as_site();
    $by = [];
    foreach (as_programs() as $p) if (!$type || $p['type'] === $type) $by[$p['type']][] = $p;
    if (!$by) return 'لا توجد برامج منشورة حاليًا في هذا القسم.';
    $lines = [$type ? 'هذه ' . ($type === 'diploma' ? 'الدبلومات' : 'الدورات') . ' المتاحة حاليًا:' : 'هذه البرامج المتاحة حاليًا في المعهد:'];
    foreach ($by as $t => $list) {
        if (!$type) $lines[] = '**' . (['diploma' => 'الدبلومات', 'qualifying' => 'الدورات التأهيلية', 'development' => 'الدورات التطويرية'][$t] ?? $t) . '**';
        foreach ($list as $p) $lines[] = "- {$p['title']}";
    }
    $lines[] = "كل البرامج: $site/programs";
    $lines[] = 'اسألني عن أي برنامج لأعطيك تفاصيله.';
    return implode("\n", $lines);
}

/* ---------- understanding ---------- */

/** Which program does the text name? Uses each title's distinctive words (not "دبلوم / دورة / إدارة"). */
function as_find_programs(array $tokens): array
{
    $progs = as_programs();
    if (!$progs) return [];
    $df = [];
    $titleTok = [];
    foreach ($progs as $p) {
        $t = array_unique(array_filter(as_tokens($p['title']), fn($x) => $x[0] !== '#'));
        $titleTok[$p['id']] = $t;
        foreach ($t as $x) $df[$x] = ($df[$x] ?? 0) + 1;
    }
    $q = array_flip($tokens);
    // typos in a program's name («المستشفيت», «الانجليزيه»): count the nearest title word
    foreach ($tokens as $t) {
        if ($t[0] === '#' || isset($df[$t]) || mb_strlen($t) < 4) continue;
        $near = as_fuzzy($t, $df);
        if ($near !== null) $q[$near] = true;
    }
    $wantType = isset($q['#diploma']) ? 'diploma' : (isset($q['#course']) ? 'course' : null);
    $scored = [];
    foreach ($progs as $p) {
        $score = 0.0;
        $total = 0.0;
        foreach ($titleTok[$p['id']] as $x) {
            $w = log(1 + count($progs) / $df[$x]);
            $total += $w;
            if (isset($q[$x])) $score += $w;
        }
        if ($score <= 0 || $total <= 0) continue;
        $ratio = $score / $total;
        if ($wantType === 'diploma' && $p['type'] === 'diploma') $ratio += 0.15;
        if ($wantType === 'course' && $p['type'] !== 'diploma') $ratio += 0.15;
        if ($wantType === 'diploma' && $p['type'] !== 'diploma') $ratio -= 0.2;
        if ($wantType === 'course' && $p['type'] === 'diploma') $ratio -= 0.2;
        $scored[] = [$ratio, $p];
    }
    usort($scored, fn($a, $b) => $b[0] <=> $a[0]);
    $scored = array_values(array_filter($scored, fn($x) => $x[0] >= 0.34));
    if (!$scored) return [];
    $best = $scored[0][0];
    // several almost equally good → ambiguous: return them all
    return array_map(fn($x) => $x[1], array_values(array_filter($scored, fn($x) => $x[0] >= $best - 0.08)));
}

/** BM25 over all documents. */
function as_rank(array $docs, array $tokens): array
{
    // "كم / كام" alone says nothing about the topic
    $tokens = array_values(array_filter($tokens, fn($t) => $t !== '#howmuch'));
    if (!$tokens) return [];
    $N = count($docs);
    $df = [];
    $lens = [];
    $tf = [];
    foreach ($docs as $i => $d) {
        $counts = [];
        $len = 0;
        foreach ($d['fields'] as [$text, $w]) {
            foreach (as_tokens((string) $text) as $t) { $counts[$t] = ($counts[$t] ?? 0) + $w; $len += $w; }
        }
        $tf[$i] = $counts;
        $lens[$i] = max(1, $len);
        foreach ($counts as $t => $_) $df[$t] = ($df[$t] ?? 0) + 1;
    }
    $avg = array_sum($lens) / max(1, $N);
    $k1 = 1.4;
    $b = 0.6;
    $uniq = array_unique($tokens);
    $out = [];
    foreach ($docs as $i => $d) {
        $score = 0.0;
        $hit = 0;
        $hitWords = 0;
        foreach ($uniq as $t) {
            $f = $tf[$i][$t] ?? 0;
            if (!$f) continue;
            $hit++;
            if ($t[0] !== '#') $hitWords++;
            $idf = log(1 + ($N - ($df[$t] ?? 0) + 0.5) / (($df[$t] ?? 0) + 0.5));
            $score += $idf * ($f * ($k1 + 1)) / ($f + $k1 * (1 - $b + $b * $lens[$i] / $avg));
        }
        if ($score <= 0) continue;
        $coverage = $hit / count($uniq);
        $out[] = ['doc' => $d, 'score' => $score * ($d['boost'] ?? 1) * (0.5 + $coverage), 'coverage' => $coverage, 'words' => $hitWords];
    }
    usort($out, fn($a, $b) => $b['score'] <=> $a['score']);
    return $out;
}

/* ---------- the conversation ---------- */

/**
 * Small talk first («ازيك»، «انت مين»…), then the real question if one is left in the message.
 * @param array $history [{role, content}] (untrusted, from the browser)
 * @return array{answer: string, suggestions: string[], confident: bool}
 */
function as_answer(string $question, array $history = [], ?array $kbOverride = null): array
{
    $question = trim(mb_substr($question, 0, AS_MAX_Q));
    $st = as_smalltalk($question);
    if ($st) {
        $restTok = array_filter(as_tokens($st['rest']), fn($t) => $t !== '#hello' && $t !== '#thanks');
        if ($st['courtesy'] !== null && $restTok && mb_strlen($st['rest']) >= 3) {
            $sub = as_answer_core($st['rest'], $history, $kbOverride);
            if ($sub['confident']) return ['answer' => trim($st['courtesy'] . "\n" . $sub['answer']), 'suggestions' => $sub['suggestions'], 'confident' => true];
        }
        return ['answer' => $st['answer'], 'suggestions' => $st['suggestions'], 'confident' => true];
    }
    return as_answer_core($question, $history, $kbOverride);
}

/** Answers that are computed when chosen: «__page:…», «__about__», «__advice__». */
function as_resolve(array $r, array $tok, array $docs): array
{
    $a = $r['answer'];
    if (!str_starts_with($a, '__')) return $r;
    if ($a === '__advice__') { $r['answer'] = as_advice($tok); return $r; }
    $want = $a === '__about__' ? null : as_norm(substr($a, 7));
    foreach ($docs as $d) {
        if ($want === null ? $d['kind'] === 'about' : ($d['kind'] === 'page' && str_contains(as_norm($d['title']), $want))) {
            $r['answer'] = $d['answer'];
            return $r;
        }
    }
    $r['answer'] = "تواصل معنا لمعرفة التفاصيل:\n" . as_contact_block();
    return $r;
}

function as_answer_core(string $question, array $history = [], ?array $kbOverride = null): array
{
    $set = as_settings();
    $site = as_site();
    $tok = as_tokens($question, false, true);
    $tags = array_flip(array_filter($tok, fn($t) => $t[0] === '#'));
    $words = array_values(array_filter($tok, fn($t) => $t[0] !== '#'));
    $has = fn(string $t) => isset($tags[$t]);
    $focus = array_keys($tags);
    $sug = fn(array $extra = []) => array_values(array_slice(array_unique(array_merge($extra, ['ما البرامج المتاحة؟', 'كيف أسجل؟', 'كيف أتواصل معكم؟'])), 0, 3));

    if ($question === '') return ['answer' => 'اكتب سؤالك وسأساعدك 🌿', 'suggestions' => $sug(), 'confident' => true];

    // greetings / thanks (only when that's all the message says)
    if ($has('#hello') && count($words) <= 1 && count($tags) === 1) {
        return ['answer' => "أهلًا وسهلًا بك 🌿 أنا **{$set['name']}**، مساعدتك في المعهد. اسألني عن البرامج، التسجيل، الرسوم، المواعيد، أو السياسات.", 'suggestions' => $sug(), 'confident' => true];
    }
    if ($has('#thanks') && count($words) <= 2) {
        return ['answer' => 'العفو، سعدت بمساعدتك 🌿 إن احتجت أي شيء آخر فأنا هنا.', 'suggestions' => $sug(), 'confident' => true];
    }

    // the dashboard's Q&A wins when it clearly matches
    $docs = as_docs($kbOverride);
    $ranked = as_rank($docs, $tok);
    $top = $ranked[0] ?? null;
    if ($top && $top['doc']['kind'] === 'kb' && $top['coverage'] >= 0.6) {
        return ['answer' => $top['doc']['answer'], 'suggestions' => as_related($ranked, $top), 'confident' => true];
    }

    // a program named in this message
    $progs = as_find_programs($tok);
    // "كم / بكام" next to a program = its price
    if ($progs && $has('#howmuch') && !$has('#duration') && !$has('#credit')) { $tags['#fees'] = true; $focus[] = '#fees'; }

    // topics that are never about one program
    if (!$progs && ($has('#place') || $has('#hours') || $has('#contact'))) {
        return ['answer' => "يسعدنا تواصلك معنا:\n" . implode("\n", as_contact_lines()), 'suggestions' => ['ما البرامج المتاحة؟', 'كيف أسجل؟', 'هل الشهادات معتمدة؟'], 'confident' => true];
    }

    // a short follow-up about the program of the previous turn: «وكم مدته؟» «بكام؟» «هل هو معتمد؟»
    $attrs = ['#fees', '#duration', '#credit', '#mode', '#cert', '#howmuch'];
    $onlyAttrs = $words === [] && $tags && !array_diff(array_keys($tags), $attrs);
    $ctx = null;
    if (!$progs && $onlyAttrs) {
        $last = as_last_program($history);
        if (count($last) === 1) {
            $progs = $last;
            if ($has('#howmuch') && !$has('#duration') && !$has('#credit')) { $tags['#fees'] = true; $focus[] = '#fees'; }
        }
    }
    if (!$progs && $has('#register')) $ctx = as_last_program($history)[0] ?? null;

    if (count($progs) > 1) {
        $lines = ['أي برنامج تقصد؟'];
        foreach ($progs as $p) $lines[] = "- {$p['title']}";
        return ['answer' => implode("\n", $lines), 'suggestions' => array_slice(array_map(fn($p) => $p['title'], $progs), 0, 3), 'confident' => true];
    }
    if (count($progs) === 1) {
        $p = $progs[0];
        $next = [];
        if (!$has('#fees')) $next[] = "كم رسوم {$p['title']}؟";
        if (!$has('#duration')) $next[] = 'كم مدته؟';
        if (!$has('#register')) $next[] = 'كيف أسجل؟';
        return ['answer' => as_program_answer($p, $focus), 'suggestions' => array_slice($next, 0, 3), 'confident' => true];
    }

    // "which program suits me?"
    if ($has('#advice')) {
        return ['answer' => as_advice($tok), 'suggestions' => array_slice(array_map(fn($p) => $p['title'], as_programs()), 0, 3), 'confident' => true];
    }

    // the built-in core FAQ when it clearly matches («هل يوجد تقسيط؟»، «أي برنامج يناسبني؟»…)
    foreach ($ranked as $r) {
        if ($r['doc']['kind'] !== 'core') continue;
        // needs a real word in common, not just a topic tag («عايزة اعرف الدبلومات» is a list, not "diploma vs course")
        $tagCount = count(array_unique(array_filter($tok, fn($t) => $t[0] === '#' && $t !== '#howmuch')));
        if ($r['coverage'] >= 0.6 && $r['score'] >= 2.5 && ($r['words'] >= 1 || ($r['coverage'] >= 0.99 && $tagCount >= 2))) {
            return as_resolve(['answer' => $r['doc']['answer'], 'suggestions' => as_related($ranked, $r), 'confident' => true], $tok, $docs);
        }
        break;
    }

    // intents answered from live data
    if ($has('#program') || (($has('#diploma') || $has('#course')) && count($words) <= 2 && !$has('#register'))) {
        $type = $has('#diploma') && !$has('#course') ? 'diploma' : null;
        $list = as_programs_list($type);
        if ($has('#course') && !$has('#diploma')) {
            $list = as_programs_list(null);
        }
        return ['answer' => $list, 'suggestions' => array_slice(array_map(fn($p) => $p['title'], as_programs()), 0, 3), 'confident' => true];
    }
    if ($has('#register') && !$has('#refund')) {
        $a = $ctx
            ? "للتسجيل في **{$ctx['title']}**:\n- عبّئ نموذج التسجيل: $site/register\n- اختر البرنامج في النموذج، وسيتواصل معك فريق القبول لإكمال الإجراءات."
            : "التسجيل سهل:\n- اختر البرنامج المناسب من صفحة البرامج: $site/programs\n- عبّئ نموذج التسجيل: $site/register\n- سيتواصل معك فريق القبول لإكمال الإجراءات.";
        if ($has('#reqs')) $a .= "\nأما الشروط والمستندات المطلوبة فيوضحها لك فريق القبول حسب البرنامج.";
        return ['answer' => $a . "\nللاستفسار:\n" . implode("\n", as_contact_lines(true)), 'suggestions' => $sug(['ما البرامج المتاحة؟']), 'confident' => true];
    }
    if ($has('#cert') && !$progs) {
        $acc = array_values(array_unique(array_filter(array_map(fn($p) => trim((string) $p['accreditation']), as_programs()))));
        $names = array_column(DB::all('SELECT name FROM accreditations WHERE is_published = 1 ORDER BY sort'), 'name');
        $lines = [];
        if (count($acc) === 1) $lines[] = "نعم، برامج المعهد معتمدة من **{$acc[0]}**.";
        elseif ($acc) { $lines[] = 'برامج المعهد معتمدة، وجهات الاعتماد:'; foreach ($acc as $x) $lines[] = "- $x"; }
        if ($names) $lines[] = 'اعتمادات المعهد: ' . implode('، ', $names);
        if (!$lines) $lines[] = 'تفاصيل الاعتماد يوضحها لك فريق القبول حسب البرنامج.';
        $lines[] = 'اذكر اسم البرنامج لأعطيك تفاصيل اعتماده.';
        return ['answer' => implode("\n", $lines), 'suggestions' => $sug(['ما البرامج المتاحة؟']), 'confident' => true];
    }
    if ($has('#portal')) {
        $a = "لمتابعة برامجك وحسابك ومدفوعاتك ادخل **بوابة المتدربين**: $site/portal";
        if ($has('#lms') && !empty(all_settings()['general']['lms_url'])) $a .= "\nومنصة التعليم الإلكتروني: " . all_settings()['general']['lms_url'];
        return ['answer' => $a, 'suggestions' => $sug(), 'confident' => true];
    }
    if ($has('#lms') && !empty(all_settings()['general']['lms_url'])) {
        return ['answer' => 'منصة التعليم الإلكتروني: ' . all_settings()['general']['lms_url'], 'suggestions' => $sug(), 'confident' => true];
    }
    if ($has('#fees') && !$progs) {
        return ['answer' => "الرسوم تختلف حسب البرنامج. اذكر اسم البرنامج وسأخبرك بما هو منشور عنه، أو تواصل مع فريق القبول لمعرفة الرسوم وخيارات السداد:\n" . implode("\n", as_contact_lines(true)), 'suggestions' => array_slice(array_map(fn($p) => "كم رسوم {$p['title']}؟", as_programs()), 0, 3), 'confident' => true];
    }

    // best document from the knowledge (pages, about, posts, Q&A…): its title must share a word with the
    // question, or the question must be well covered — otherwise a stray word ("مدير") would pick a random page
    $titleHit = fn(array $r) => (bool) array_intersect(as_tokens($r['doc']['title']), $tok);
    $topical = array_unique(array_filter($tok, fn($t) => $t !== '#howmuch'));
    if ($top && (($titleHit($top) && $top['coverage'] >= 0.5) || ($top['coverage'] >= 0.75 && $top['score'] >= 5 && count($topical) >= 2))) {
        return as_resolve(['answer' => $top['doc']['answer'], 'suggestions' => as_related($ranked, $top), 'confident' => true], $tok, $docs);
    }

    // not sure: say so, offer the closest topics and a human
    $related = array_slice(array_map(fn($r) => $r['doc']['title'], array_filter($ranked, fn($r) => $r['score'] > 1)), 0, 3);
    $a = $set['fallback'] . "\nيمكنك سؤال فريق القبول مباشرة:\n" . implode("\n", as_contact_lines(true));
    if ($related) $a = $set['fallback'] . "\nقد يفيدك أحد هذه المواضيع، أو تواصل مع فريق القبول:\n" . implode("\n", as_contact_lines(true));
    return ['answer' => $a, 'suggestions' => $related ? array_values(array_unique($related)) : $sug(), 'confident' => false];
}

function as_related(array $ranked, array $top): array
{
    $out = [];
    foreach ($ranked as $r) {
        if ($r['doc']['id'] === $top['doc']['id'] || $r['doc']['title'] === $top['doc']['title']) continue;
        if ($r['score'] < 1.5) break;
        $out[$r['doc']['title']] = true;
        if (count($out) >= 3) break;
    }
    return array_keys($out) ?: ['ما البرامج المتاحة؟', 'كيف أسجل؟', 'كيف أتواصل معكم؟'];
}

/** The program the conversation was about (last assistant / user message naming exactly one). */
function as_last_program(array $history): array
{
    foreach (array_reverse(array_slice($history, -6)) as $m) {
        $found = as_find_programs(as_tokens((string) ($m['content'] ?? '')));
        if (count($found) === 1) return $found;
        // a program card the assistant sent starts with **title**
        if (($m['role'] ?? '') === 'assistant' && preg_match('/^\*\*(.+?)\*\*/u', (string) $m['content'], $mm)) {
            foreach (as_programs() as $p) if ($p['title'] === $mm[1]) return [$p];
        }
    }
    return [];
}
