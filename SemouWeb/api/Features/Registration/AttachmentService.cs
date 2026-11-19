using System.Data.Common;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Registration;

/*
 * المرفقات (AttachForm): the registration cannot be saved without the required files.
 * Files are forwarded to the same archive server the desktop uses (Archive:UploadUrl), signed with the
 * same HMAC token (Archive:UploadSecret, kept in user-secrets). The server remembers, per user and
 * national id, which codes were uploaded, so the save can verify it (the desktop only had a checkbox).
 */
public sealed record AttachmentRequirement(string Code, string Name, bool Required, string? Rule);

public sealed class AttachmentService(IConfiguration config, IHttpClientFactory http, IMemoryCache cache, CurrentUser me, ILogger<AttachmentService> log)
{
    private const string OfficeCash = "صندوق المكتب";
    private static readonly HashSet<string> AllowedExt = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".webp", ".heic" };
    private const long MaxBytes = 10 * 1024 * 1024;
    private static readonly char[] InvalidNameChars = [.. Path.GetInvalidFileNameChars(), '/', (char)92, ':', '*', '?', '"', '<', '>', '|', '.'];

    /// <summary>ShowAttachName (+ the AttachForm "required" rules, evaluated later with the payment details).</summary>
    public async Task<IEnumerable<AttachmentRequirement>> RequirementsAsync(DbConnection c, int regType)
    {
        // Desktop: courses (type 1) only show type_ = 1 attachments; diplomas show all.
        var rows = await c.QueryAsync<(string AttachName, string Code)>(
            "SELECT AttachName, Code FROM dbo.AttachName WHERE Is_Use = 1 AND (@t IS NULL OR type_ = @t) ORDER BY ID", new { t = regType == 1 ? 1 : (int?)null });
        return rows.Select(r => r.Code switch
        {
            "Letter" => new AttachmentRequirement(r.Code, r.AttachName, false, null),
            "contract" => new AttachmentRequirement(r.Code, r.AttachName, false, "مطلوب إذا وقع المتدرب اتفاقية التدريب"),
            "trans" => new AttachmentRequirement(r.Code, r.AttachName, true, "غير مطلوب عند السداد نقداً من صندوق المكتب"),
            "trans2" => new AttachmentRequirement(r.Code, r.AttachName, false, "مطلوب عند تقسيم السداد على بنك ثانٍ"),
            _ => new AttachmentRequirement(r.Code, r.AttachName, true, null),
        });
    }

    private static bool IsRequired(string code, int regType, bool contract, string? cashName, string? cashName2) => code switch
    {
        "Letter" => false,
        "contract" => contract && regType == 0,
        "trans" => cashName != null && cashName != OfficeCash,
        "trans2" => cashName2 != null && cashName2 != OfficeCash,
        "Certifcat" => regType == 0,
        "ID" => true,
        _ => false,
    };

    private string Key(string nationalId) => $"attach:{me.UserGuid}:{nationalId}";

    public IReadOnlyCollection<string> Uploaded(string nationalId) =>
        cache.TryGetValue(Key(nationalId), out HashSet<string>? s) ? s! : [];

    public void EnsureUploaded(string nationalId, int regType, bool contract, string? cashName, string? cashName2)
    {
        var have = Uploaded(nationalId);
        foreach (var code in new[] { "ID", "Certifcat", "trans", "trans2", "contract" })
            if (IsRequired(code, regType, contract, cashName, cashName2) && !have.Contains(code))
                throw new ApiException("لا يمكن التسجيل بدون إدخال المرفقات الخاصة بالتسجيل");
    }

    private sealed record SavedItem(string? origName, string? url);

    private (string Url, string Secret) Config()
    {
        var url = config["Archive:UploadUrl"];
        var secret = config["Archive:UploadSecret"];
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(secret)) throw new ApiException("خدمة رفع المرفقات غير مضبوطة على الخادم");
        return (url, secret);
    }

    private static string CleanId(string nationalId)
    {
        var id = new string(nationalId.Where(char.IsDigit).ToArray());
        return id.Length == 10 ? id : throw new ApiException("رقم الهوية غير صحيح");
    }

    private static void CheckFile(IFormFile f)
    {
        if (!AllowedExt.Contains(Path.GetExtension(f.FileName))) throw new ApiException($"نوع الملف غير مسموح ({f.FileName}). المسموح: PDF أو صورة");
        if (f.Length is 0 or > MaxBytes) throw new ApiException($"حجم الملف غير مسموح ({f.FileName}). الحد الأقصى 10 ميجابايت");
    }

    /// <summary>POST multipart "files[]" + HMAC token to the archive server (UploadFilesAsync). Returns the saved items.</summary>
    private async Task<List<SavedItem>> ForwardAsync(string id, IReadOnlyList<(string ServerName, IFormFile File)> files, CancellationToken ct)
    {
        if (config.GetValue("App:TestMode", false))
        {
            // Test copy: never send files to the real archive server; pretend they were saved.
            log.LogInformation("TEST MODE: skipped uploading {Count} file(s) for {Id}", files.Count, id);
            return files.Select(f => new SavedItem(f.ServerName, null)).ToList();
        }
        var (url, secret) = Config();
        using var form = new MultipartFormDataContent { { new StringContent(Token(id, secret, 30), Encoding.UTF8), "token" } };
        var streams = new List<Stream>();
        try
        {
            foreach (var (name, f) in files)
            {
                var s = f.OpenReadStream(); streams.Add(s);
                var content = new StreamContent(s);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(content, "files[]", name);
            }
            using var client = http.CreateClient("archive-upload");
            using var resp = await client.PostAsync(url, form, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                log.LogWarning("Attachment upload failed {Status}: {Body}", resp.StatusCode, body);
                throw new ApiException("فشل رفع المرفقات على خادم الأرشيف، حاول مرة أخرى", 502);
            }
            using var doc = JsonDocument.Parse(body);
            var ok = doc.RootElement.TryGetProperty("success", out var sp) && sp.ValueKind == JsonValueKind.True;
            var saved = doc.RootElement.TryGetProperty("saved", out var sv) && sv.ValueKind == JsonValueKind.Array
                ? sv.EnumerateArray().Select(x => new SavedItem(x.TryGetProperty("origName", out var o) ? o.GetString() : null, x.TryGetProperty("url", out var u) ? u.GetString() : null)).ToList()
                : [];
            if (!ok || saved.Count == 0)
                throw new ApiException(doc.RootElement.TryGetProperty("message", out var m) ? $"فشل رفع المرفقات: {m.GetString()}" : "فشل رفع المرفقات", 502);
            return saved;
        }
        catch (HttpRequestException ex)
        {
            log.LogWarning(ex, "Attachment upload error");
            throw new ApiException("تعذر الاتصال بخادم الأرشيف لرفع المرفقات", 502);
        }
        finally { foreach (var s in streams) s.Dispose(); }
    }

    /// <summary>Registration attachments, named {Code}_{NationalId}{ext} like the desktop AttachForm.</summary>
    public async Task<object> UploadAsync(string nationalId, IReadOnlyList<(string Code, IFormFile File)> files, CancellationToken ct)
    {
        var id = CleanId(nationalId);
        if (files.Count == 0) throw new ApiException("لا يمكن الحفظ بدون اختيار الملفات المرفقة");
        foreach (var (code, f) in files)
        {
            CheckFile(f);
            if (code.Length == 0 || code.Any(ch => !char.IsLetterOrDigit(ch))) throw new ApiException("نوع المرفق غير صحيح");
        }
        var named = files.Select(x => ($"{x.Code}_{id}{Path.GetExtension(x.File.FileName).ToLowerInvariant()}", x.File)).ToList();
        var saved = await ForwardAsync(id, named, ct);
        var set = cache.GetOrCreate(Key(id), e => { e.SlidingExpiration = TimeSpan.FromHours(2); return new HashSet<string>(); })!;
        foreach (var (code, f) in files)
            if (saved.Any(x => string.Equals(x.origName, $"{code}_{id}{Path.GetExtension(f.FileName).ToLowerInvariant()}", StringComparison.OrdinalIgnoreCase))) set.Add(code);
        return new { uploaded = set.ToArray(), saved };
    }

    /// <summary>Files with a server-decided base name (e.g. transfer images {CashName}_{BillCode}_{ddMMyyyy}).</summary>
    public async Task<object> UploadNamedAsync(string nationalId, IReadOnlyList<(string Name, IFormFile File)> files, CancellationToken ct)
    {
        var id = CleanId(nationalId);
        foreach (var (name, f) in files)
        {
            CheckFile(f);
            if (name.Length is 0 or > 120 || name.IndexOfAny(InvalidNameChars) >= 0) throw new ApiException("اسم الملف غير صحيح");
        }
        var saved = await ForwardAsync(id, files.Select(x => (x.Name + Path.GetExtension(x.File.FileName).ToLowerInvariant(), x.File)).ToList(), ct);
        return new { saved };
    }

    /// <summary>token = b64url("nationalId|exp|sig"), sig = b64url(HMACSHA256("nationalId|exp", secret)) — same as the desktop.</summary>
    private static string Token(string nationalId, string secret, int minutes)
    {
        var exp = DateTimeOffset.UtcNow.AddMinutes(minutes).ToUnixTimeSeconds();
        var payload = $"{nationalId}|{exp}";
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var sig = B64(h.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        return B64(Encoding.UTF8.GetBytes($"{payload}|{sig}"));
    }

    private static string B64(byte[] d) => Convert.ToBase64String(d).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
