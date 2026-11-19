using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Semou.Api.Features.Website;

/// <summary>Settings: "Website" section (Secret comes from user-secrets / Website__Secret env var, never git).</summary>
public sealed class WebsiteOptions
{
    public bool Enabled { get; set; }
    /// <summary>The alsmow.com site, e.g. https://new.alsmow.com (the PHP API lives under /api).</summary>
    public string BaseUrl { get; set; } = "";
    /// <summary>Shared HMAC secret, same value as erp_secret in the site's api/config.php.</summary>
    public string Secret { get; set; } = "";
    public int PullSeconds { get; set; } = 60;
    public int PushMinutes { get; set; } = 15;
}

/// <summary>
/// Calls the website's /api/erp/* endpoints. Every request is signed:
///   X-Alsmow-Expires   = unix time + 120s
///   X-Alsmow-Signature = base64url(HMAC_SHA256("METHOD|/api/path|query|expires|sha256hex(body)", Secret))
/// (verified by erp_verify() in the site's api/lib/erp.php).
/// </summary>
public sealed class WebsiteClient(HttpClient http, Microsoft.Extensions.Options.IOptions<WebsiteOptions> options)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    private WebsiteOptions O => options.Value;

    public bool Configured => O.Enabled && O.BaseUrl.Length > 0 && O.Secret.Length >= 32;

    public Task<T> GetAsync<T>(string path, string query, CancellationToken ct) => SendAsync<T>(HttpMethod.Get, path, query, null, ct);

    public Task<T> PostAsync<T>(string path, object body, CancellationToken ct) => SendAsync<T>(HttpMethod.Post, path, "", body, ct);

    private async Task<T> SendAsync<T>(HttpMethod method, string path, string query, object? body, CancellationToken ct)
    {
        var bodyBytes = body is null ? [] : JsonSerializer.SerializeToUtf8Bytes(body, Json);
        var expires = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 120;
        var canonical = $"{method.Method}|{path}|{query}|{expires}|{Convert.ToHexString(SHA256.HashData(bodyBytes)).ToLowerInvariant()}";
        var sig = Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(O.Secret), Encoding.UTF8.GetBytes(canonical)))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        using var req = new HttpRequestMessage(method, O.BaseUrl.TrimEnd('/') + path + (query.Length > 0 ? "?" + query : ""));
        req.Headers.Add("X-Alsmow-Expires", expires.ToString());
        req.Headers.Add("X-Alsmow-Signature", sig);
        if (body is not null)
        {
            req.Content = new ByteArrayContent(bodyBytes);
            req.Content.Headers.ContentType = new("application/json") { CharSet = "utf-8" };
        }
        using var res = await http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"website {method} {path} -> {(int)res.StatusCode}: {await res.Content.ReadAsStringAsync(ct)}");
        return (await res.Content.ReadFromJsonAsync<T>(Json, ct))!;
    }
}
