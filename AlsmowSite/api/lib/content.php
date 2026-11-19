<?php
declare(strict_types=1);

/* ============================================================
   Content safety: HTML allowlist sanitizer + image uploads.
   ============================================================ */

const HTML_ALLOWED = [
    'p' => [], 'br' => [], 'strong' => [], 'b' => [], 'em' => [], 'i' => [], 'u' => [], 's' => [],
    'h2' => [], 'h3' => [], 'h4' => [], 'ul' => [], 'ol' => [], 'li' => [], 'blockquote' => [],
    'hr' => [], 'code' => [], 'pre' => [], 'mark' => [],
    'a' => ['href', 'target', 'rel'],
    'img' => ['src', 'alt'],
    'table' => [], 'thead' => [], 'tbody' => [], 'tr' => [], 'th' => [], 'td' => [],
];

function safe_url(string $url, bool $allowContact = true): ?string
{
    $url = trim($url);
    if ($url === '') return null;
    if (str_starts_with($url, '/') && !str_starts_with($url, '//')) return $url;
    $scheme = strtolower((string) parse_url($url, PHP_URL_SCHEME));
    $ok = ['http', 'https'];
    if ($allowContact) array_push($ok, 'mailto', 'tel');
    return in_array($scheme, $ok, true) ? $url : null;
}

function sanitize_html(?string $html): ?string
{
    if ($html === null || trim($html) === '') return null;
    $doc = new DOMDocument('1.0', 'UTF-8');
    libxml_use_internal_errors(true);
    $doc->loadHTML('<?xml encoding="utf-8"?><div id="__root">' . $html . '</div>', LIBXML_HTML_NOIMPLIED | LIBXML_HTML_NODEFDTD);
    libxml_clear_errors();
    $root = $doc->getElementById('__root');
    if (!$root) return null;
    clean_node($root);
    $out = '';
    foreach ($root->childNodes as $child) $out .= $doc->saveHTML($child);
    // "<p></p>" from an empty editor counts as no content
    $hasText = trim(html_entity_decode(strip_tags($out)), " \t\n\r\0\x0B\u{A0}") !== '';
    return $hasText || str_contains($out, '<img') || str_contains($out, '<hr') ? $out : null;
}

function clean_node(DOMNode $node): void
{
    // iterate over a static copy: we mutate the tree while walking
    foreach (iterator_to_array($node->childNodes) as $child) {
        if ($child instanceof DOMText) continue;
        if (!$child instanceof DOMElement) { $node->removeChild($child); continue; }

        $tag = strtolower($child->tagName);
        if (in_array($tag, ['script', 'style', 'iframe', 'object', 'embed', 'form', 'input', 'svg', 'math'], true)) {
            $node->removeChild($child);
            continue;
        }
        if (!array_key_exists($tag, HTML_ALLOWED)) {
            // unknown wrapper (div, span, font...): keep its children, drop the tag
            clean_node($child);
            while ($child->firstChild) $node->insertBefore($child->firstChild, $child);
            $node->removeChild($child);
            continue;
        }

        $align = null;
        if (preg_match('/text-align:\s*(right|left|center|justify)/i', $child->getAttribute('style'), $m)) {
            $align = strtolower($m[1]);
        }
        foreach (iterator_to_array($child->attributes) as $attr) {
            if (!in_array($attr->name, HTML_ALLOWED[$tag], true)) $child->removeAttribute($attr->name);
        }
        if ($align && in_array($tag, ['p', 'h2', 'h3', 'h4'], true)) $child->setAttribute('style', "text-align: $align");

        if ($tag === 'a') {
            $href = safe_url($child->getAttribute('href'));
            if ($href === null) $child->removeAttribute('href'); else $child->setAttribute('href', $href);
            if ($child->getAttribute('target') === '_blank') $child->setAttribute('rel', 'noopener noreferrer');
            else $child->removeAttribute('target');
        }
        if ($tag === 'img') {
            $src = safe_url($child->getAttribute('src'), false);
            if ($src === null) { $node->removeChild($child); continue; }
            $child->setAttribute('src', $src);
            $child->setAttribute('loading', 'lazy');
        }
        clean_node($child);
    }
}

function plain_text(mixed $v, int $max): ?string
{
    if ($v === null) return null;
    $s = trim(strip_tags((string) $v));
    $s = preg_replace('/[^\S\n]+/u', ' ', $s) ?? $s;
    if ($s === '') return null;
    return mb_substr($s, 0, $max);
}

/* ---------------- Uploads ---------------- */

const UPLOAD_MAX_BYTES = 8 * 1024 * 1024;
const UPLOAD_MAX_WIDTH = 1920;

/**
 * Accepts jpeg/png/webp/gif, decodes it with GD and re-encodes it as WebP
 * (resized to <= 1920px wide). Re-encoding strips EXIF/GPS data and any
 * payload hidden inside the file. SVG is refused on purpose (can carry scripts).
 */
function store_upload(array $file, array $user): array
{
    if (($file['error'] ?? UPLOAD_ERR_NO_FILE) !== UPLOAD_ERR_OK) {
        throw new HttpError(422, ($file['error'] ?? 0) === UPLOAD_ERR_INI_SIZE ? 'حجم الصورة أكبر من المسموح' : 'لم يتم استلام الملف');
    }
    if ($file['size'] > UPLOAD_MAX_BYTES) throw new HttpError(422, 'حجم الصورة يجب ألا يزيد عن 8 ميجابايت');
    return store_image_file($file['tmp_name'], (string) $file['name'], (int) $user['id']);
}

function store_image_file(string $path, string $originalName, ?int $userId): array
{
    $mime = (new finfo(FILEINFO_MIME_TYPE))->file($path);
    $img = match ($mime) {
        'image/jpeg' => @imagecreatefromjpeg($path),
        'image/png' => @imagecreatefrompng($path),
        'image/webp' => @imagecreatefromwebp($path),
        'image/gif' => @imagecreatefromgif($path),
        default => throw new HttpError(422, 'نوع الملف غير مدعوم، المسموح: JPG و PNG و WebP'),
    };
    if (!$img) throw new HttpError(422, 'تعذر قراءة الصورة، قد يكون الملف تالفًا');

    if ($mime === 'image/jpeg') $img = apply_exif_orientation($img, $path);

    $w = imagesx($img);
    $h = imagesy($img);
    if ($w > UPLOAD_MAX_WIDTH) {
        $nh = (int) round($h * UPLOAD_MAX_WIDTH / $w);
        $resized = imagecreatetruecolor(UPLOAD_MAX_WIDTH, $nh);
        imagealphablending($resized, false);
        imagesavealpha($resized, true);
        imagecopyresampled($resized, $img, 0, 0, 0, 0, UPLOAD_MAX_WIDTH, $nh, $w, $h);
        imagedestroy($img);
        $img = $resized;
        [$w, $h] = [UPLOAD_MAX_WIDTH, $nh];
    } else {
        imagepalettetotruecolor($img);
        imagealphablending($img, false);
        imagesavealpha($img, true);
    }

    $sub = date('Y/m');
    $dir = rtrim(config('uploads_dir'), '/') . '/' . $sub;
    if (!is_dir($dir) && !mkdir($dir, 0755, true)) throw new HttpError(500, 'تعذر إنشاء مجلد الرفع');
    $name = bin2hex(random_bytes(10)) . '.webp';
    if (!imagewebp($img, "$dir/$name", 82)) throw new HttpError(500, 'تعذر حفظ الصورة');
    imagedestroy($img);

    $url = rtrim(config('uploads_url'), '/') . "/$sub/$name";
    $size = filesize("$dir/$name") ?: null;
    $id = DB::insert('media', [
        'url' => $url,
        'original_name' => mb_substr(basename($originalName), 0, 250),
        'width' => $w,
        'height' => $h,
        'size' => $size,
        'created_by' => $userId,
    ]);
    return ['id' => $id, 'url' => $url, 'width' => $w, 'height' => $h, 'size' => $size];
}

function apply_exif_orientation(GdImage $img, string $path): GdImage
{
    if (!function_exists('exif_read_data')) return $img;
    $exif = @exif_read_data($path);
    $angle = match ((int) ($exif['Orientation'] ?? 1)) { 3 => 180, 6 => -90, 8 => 90, default => 0 };
    if ($angle === 0) return $img;
    $rotated = imagerotate($img, $angle, 0);
    return $rotated ?: $img;
}

function delete_media_file(string $url): void
{
    $prefix = rtrim(config('uploads_url'), '/') . '/';
    if (!str_starts_with($url, $prefix)) return;
    $rel = substr($url, strlen($prefix));
    if (!preg_match('#^\d{4}/\d{2}/[a-f0-9]{20}\.webp$#', $rel)) return; // never follow anything else
    $path = rtrim(config('uploads_dir'), '/') . '/' . $rel;
    if (is_file($path)) @unlink($path);
}
