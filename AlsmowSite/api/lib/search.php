<?php
declare(strict_types=1);

/* ============================================================
   Dashboard search: "where do I edit this text?"
   Looks in every site setting (label + current value) and every text
   column of every content type (incl. full HTML bodies), and returns
   where each hit lives so the dashboard can open that exact place.
   ============================================================ */

const SEARCH_TEXT_TYPES = ['string', 'text', 'html', 'list', 'url', 'stats', 'slug'];

/** Plain text of a stored value (HTML stripped, lists/stats flattened). */
function search_plain($v): string
{
    if ($v === null) return '';
    if (is_array($v)) {
        $parts = [];
        array_walk_recursive($v, function ($x) use (&$parts) { if (is_scalar($x)) $parts[] = (string) $x; });
        return implode(' • ', $parts);
    }
    $s = (string) $v;
    if ($s !== '' && ($s[0] === '[' || $s[0] === '{')) {
        $j = json_decode($s, true);
        if (is_array($j)) return search_plain($j);
    }
    $s = html_entity_decode(strip_tags(preg_replace('/<(br|\/p|\/li|\/h\d)\b[^>]*>/i', ' $0', $s)), ENT_QUOTES | ENT_HTML5, 'UTF-8');
    return trim(preg_replace('/\s+/u', ' ', $s));
}

/** Arabic-friendly normalisation so أ/إ/آ/ا, ة/ه, ى/ي and diacritics all match. */
function search_norm(string $s): string
{
    $s = preg_replace('/[\x{064B}-\x{065F}\x{0670}\x{0640}]/u', '', $s); // harakat + tatweel
    $s = strtr($s, ['أ' => 'ا', 'إ' => 'ا', 'آ' => 'ا', 'ٱ' => 'ا', 'ة' => 'ه', 'ى' => 'ي', 'ؤ' => 'و', 'ئ' => 'ي']);
    return mb_strtolower($s, 'UTF-8');
}

/** A short excerpt around the first match (null when there is no match). */
function search_snippet(string $text, string $nq, int $around = 48): ?string
{
    if ($text === '') return null;
    $pos = mb_strpos(search_norm($text), $nq, 0, 'UTF-8');
    if ($pos === false) return null;
    $len = mb_strlen($text, 'UTF-8');
    $from = max(0, $pos - $around);
    $to = min($len, $pos + mb_strlen($nq, 'UTF-8') + $around);
    return ($from > 0 ? '…' : '') . mb_substr($text, $from, $to - $from, 'UTF-8') . ($to < $len ? '…' : '');
}

function admin_search(string $q): array
{
    $q = trim(preg_replace('/\s+/u', ' ', $q));
    if (mb_strlen($q, 'UTF-8') < 2) return ['q' => $q, 'settings' => [], 'content' => []];
    $nq = search_norm($q);

    // 1) settings: field label or current value
    $settings = [];
    $values = all_settings();
    foreach (settings_schema() as $group => $g) {
        foreach ($g['fields'] as $key => $f) {
            $val = search_plain($values[$group][$key] ?? null);
            $inValue = search_snippet($val, $nq);
            $inLabel = str_contains(search_norm($f['label'] . ' ' . ($f['help'] ?? '') . ' ' . ($f['placeholder'] ?? '')), $nq);
            if ($inValue === null && !$inLabel) continue;
            $settings[] = [
                'group' => $group, 'groupLabel' => $g['label'], 'field' => $key, 'label' => $f['label'],
                'snippet' => $inValue ?? ($val !== '' ? mb_substr($val, 0, 100, 'UTF-8') : ''), 'byValue' => $inValue !== null,
            ];
        }
    }
    usort($settings, fn($a, $b) => $b['byValue'] <=> $a['byValue']);

    // 2) content: every text column, narrowed in SQL first, then matched precisely in PHP
    $content = [];
    foreach (resources() as $name => $res) {
        $cols = array_keys(array_filter($res['fields'], fn($f) => in_array($f['type'], SEARCH_TEXT_TYPES, true)));
        if (!$cols) continue;
        // SQL LIKE on the raw words (MySQL utf8mb4 collations already ignore case; the PHP pass handles hamza/ta-marbuta)
        $words = array_slice(array_filter(explode(' ', $q), fn($w) => mb_strlen($w, 'UTF-8') >= 2), 0, 4);
        $loose = preg_replace('/[أإآاٱ]/u', '_', $words[0] ?? $q);  // any alef form
        $loose = preg_replace('/[ةه]/u', '_', $loose);
        $loose = preg_replace('/[ىي]/u', '_', $loose);
        $where = '(' . implode(' OR ', array_map(fn($c) => "`$c` LIKE ?", $cols)) . ')';
        $params = array_fill(0, count($cols), '%' . $loose . '%');
        $rows = DB::all("SELECT * FROM `{$res['table']}` WHERE $where ORDER BY {$res['order']} LIMIT 60", $params);
        foreach ($rows as $row) {
            foreach ($cols as $c) {
                $snip = search_snippet(search_plain($row[$c] ?? null), $nq);
                if ($snip === null) continue;
                $content[] = [
                    'res' => $name, 'resLabel' => $res['label'], 'id' => (int) $row['id'],
                    'title' => search_plain($row[$res['title']] ?? '') ?: ('#' . $row['id']),
                    'field' => $c, 'label' => $res['fields'][$c]['label'], 'snippet' => $snip,
                    'published' => isset($row['is_published']) ? (bool) $row['is_published'] : null,
                ];
                break; // one hit per row: the first field that matches
            }
            if (count($content) >= 60) break 2;
        }
    }

    return ['q' => $q, 'settings' => array_slice($settings, 0, 40), 'content' => $content];
}
