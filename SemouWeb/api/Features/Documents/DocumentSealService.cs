using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Documents;

/*
 * توثيق المستندات: every printed document carries a verification code + QR that opens /verify/{type}/{guid}?c=CODE.
 * CODE = HMAC-SHA256(Docs:SealSecret, type|guid|number|date|amount|party) → 60 bits, Crockford base32 (XXXX-XXXX-XXXX).
 * The code is computed from the CURRENT database row, so:
 *   - a forged paper (unknown guid / made-up code) → "غير موثق"
 *   - a real paper whose amount/name/date was changed by hand → the QR still opens, but the page shows the true data
 *   - a document edited or cancelled in the system after printing → "تم تعديله / ملغي"
 * Nothing secret is stored; the secret lives in user-secrets (Docs:SealSecret).
 */
public sealed record DocFacts(string Type, string KindName, string Number, DateTime Date, decimal Amount, string? Party, string? PartyId, string? Branch, bool Cancelled);

public sealed class DocumentSealService(Db db, IConfiguration config)
{
    public const string Issuer = "معهد السمو المتخصص العالي للتدريب";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

    public static readonly IReadOnlyDictionary<string, string> Types = new Dictionary<string, string>
    {
        ["inv"] = "فاتورة ضريبية",
        ["reg"] = "استمارة تسجيل",
        ["cash"] = "سند",
        ["jv"] = "قيد يومية",
        ["st"] = "كشف حساب طالب",
        ["fee"] = "استمارة رسوم",
        ["rreg"] = "مرتجع تسجيل",
        ["rbill"] = "مرتجع مبيعات",
        ["disc"] = "سند خصم",
    };

    private byte[] Secret()
    {
        var s = config["Docs:SealSecret"];
        if (string.IsNullOrEmpty(s) || s.Length < 32) throw new ApiException("توثيق المستندات غير مضبوط على الخادم (Docs:SealSecret)");
        return Encoding.UTF8.GetBytes(s);
    }

    public async Task<DocFacts?> FactsAsync(string type, Guid guid, CancellationToken ct)
    {
        if (!Types.ContainsKey(type)) return null;
        await using var c = await db.OpenAsync(ct);
        var sql = type switch
        {
            "inv" => """
                SELECT B.Code AS Number, B.BillDate AS Date, CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS Amount, S.StudentName AS Party, S.NationalId AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(B.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.Bill B LEFT JOIN dbo.Student S ON S.AccountGuid = B.AccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = B.BranchGuid WHERE B.Guid = @guid
                """,
            "reg" => """
                SELECT R.Code AS Number, R.RegDate AS Date, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS Amount, S.StudentName AS Party, S.NationalId AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(R.IstUse,1) = 0 OR ISNULL(R.IsReReg,0) = 1 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.RegDoc R LEFT JOIN dbo.Student S ON S.AccountGuid = R.AccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = R.BranchGuid WHERE R.Guid = @guid
                """,
            "cash" => """
                SELECT C.Code AS Number, C.CashDayDate AS Date, CAST(ISNULL(C.Amount,0) AS decimal(18,2)) AS Amount, ISNULL(NULLIF(C.VendorName,''), A.AccountName) AS Party, CAST(NULL AS nvarchar(20)) AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(C.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, ISNULL(C.CashDayType,0) AS Sub
                FROM dbo.CashDay C LEFT JOIN dbo.Account A ON A.Guid = C.FAccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = C.BranchGuid WHERE C.Guid = @guid
                """,
            "fee" => """
                SELECT F.Code AS Number, F.RegFessDate AS Date, CAST(ISNULL(F.ActionSubTotal,0) AS decimal(18,2)) AS Amount, S.StudentName AS Party, S.NationalId AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(F.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.RegOtherFess F LEFT JOIN dbo.Student S ON S.AccountGuid = F.AccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = F.BranchGuid WHERE F.Guid = @guid
                """,
            "rreg" or "rbill" => $"""
                SELECT X.Code AS Number, X.ReDate AS Date, CAST(ISNULL(X.ReSubTotal,0) AS decimal(18,2)) AS Amount, S.StudentName AS Party, S.NationalId AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(X.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.{(type == "rreg" ? "ReRegDoc" : "ReBill")} X LEFT JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = X.BranchGuid WHERE X.Guid = @guid
                """,
            "disc" => """
                SELECT X.Code AS Number, X.ActionDate AS Date, CAST(ISNULL(X.Amount,0) AS decimal(18,2)) AS Amount, S.StudentName AS Party, S.NationalId AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(X.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.StudentDiscount X LEFT JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid LEFT JOIN dbo.Branch BR ON BR.Guid = X.BranchGuid WHERE X.Guid = @guid
                """,
            // Statement (guid = student AccountGuid): valid while the account has no new movement.
            "st" => """
                SELECT CONCAT(S.Code, '-', (SELECT COUNT(*) FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid WHERE X.AccountGuid = S.AccountGuid AND Y.IsUse = 1)) AS Number,
                       ISNULL((SELECT MAX(Y.DayDate) FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid WHERE X.AccountGuid = S.AccountGuid AND Y.IsUse = 1), CAST('2000-01-01' AS datetime)) AS Date,
                       CAST(ISNULL((SELECT SUM(ISNULL(X.Maden,0) - ISNULL(X.Daen,0)) FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid LEFT JOIN dbo.Form_Name F ON F.Guid = Y.TypeGuid
                                    WHERE X.AccountGuid = S.AccountGuid AND Y.IsUse = 1 AND ISNULL(F.UnqName,'') NOT IN ('salesinvoice','repay')),0) AS decimal(18,2)) AS Amount,
                       S.StudentName AS Party, S.NationalId AS PartyId, BR.BrName AS Branch, CAST(0 AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.Student S LEFT JOIN dbo.Branch BR ON BR.Guid = S.BarnchGuid WHERE S.AccountGuid = @guid
                """,
            _ => """
                SELECT D.Code AS Number, D.DayDate AS Date,
                       CAST(ISNULL((SELECT SUM(ISNULL(X.Maden,0)) FROM dbo.Day2 X WHERE X.Day1Guid = D.Guid AND ISNULL(X.IsUse,1) = 1),0) AS decimal(18,2)) AS Amount,
                       CAST(NULL AS nvarchar(200)) AS Party, CAST(NULL AS nvarchar(20)) AS PartyId,
                       BR.BrName AS Branch, CAST(CASE WHEN ISNULL(D.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled, CAST(0 AS int) AS Sub
                FROM dbo.Day1 D LEFT JOIN dbo.Branch BR ON BR.Guid = D.BranchGuid WHERE D.Guid = @guid
                """,
        };
        var r = await c.QueryFirstOrDefaultAsync<(string Number, DateTime Date, decimal Amount, string? Party, string? PartyId, string? Branch, bool Cancelled, int Sub)>(
            new CommandDefinition(sql, new { guid }, cancellationToken: ct));
        if (r.Number is null) return null;
        var kind = type == "cash" ? (r.Sub == 1 ? "سند صرف" : "سند قبض") : Types[type];
        return new DocFacts(type, kind, r.Number.Trim(), r.Date, r.Amount, r.Party?.Trim(), r.PartyId?.Trim(), r.Branch, r.Cancelled);
    }

    /// <summary>The verification code for the document as it is now.</summary>
    public string Code(Guid guid, DocFacts f)
    {
        var payload = string.Join('|', f.Type, guid.ToString("N"), f.Number, f.Date.ToString("yyyyMMddHHmm", Inv), f.Amount.ToString("0.00", Inv), f.PartyId ?? f.Party ?? "");
        using var h = new HMACSHA256(Secret());
        var hash = h.ComputeHash(Encoding.UTF8.GetBytes(payload));
        ulong v = 0;
        for (var i = 0; i < 8; i++) v = (v << 8) | hash[i];
        var sb = new StringBuilder();
        for (var i = 0; i < 12; i++) { sb.Append(Alphabet[(int)(v >> 59)]); v <<= 5; if (i is 3 or 7) sb.Append('-'); }
        return sb.ToString();
    }

    private static string Normalize(string? code) => new string((code ?? "").ToUpperInvariant().Where(char.IsLetterOrDigit).ToArray())
        .Replace('O', '0').Replace('I', '1').Replace('L', '1');

    public async Task<object> SealAsync(string type, Guid guid, string printedBy, CancellationToken ct)
    {
        var f = await FactsAsync(type, guid, ct) ?? throw ApiException.NotFound("المستند غير موجود");
        var code = Code(guid, f);
        return new { code, path = $"/verify/{type}/{guid}?c={code}", issuer = Issuer, cancelled = f.Cancelled, printedBy };
    }

    /// <summary>Public check. Never confirms anything without the right code (no data leaks from a bare guid).</summary>
    public async Task<object> VerifyAsync(string type, Guid guid, string? code, CancellationToken ct)
    {
        var f = await FactsAsync(type, guid, ct);
        if (f is null || string.IsNullOrWhiteSpace(code)) return new { status = "invalid", issuer = Issuer };
        var now = Code(guid, f);
        var given = Normalize(code);
        var match = CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(Normalize(now)), Encoding.ASCII.GetBytes(given));
        if (!match)
        {
            // A genuine document whose data changed after printing still has a real guid; we can't tell it from a forgery
            // by the code alone, so we only say the printed copy doesn't match and show nothing else.
            return new { status = "mismatch", issuer = Issuer };
        }
        return new
        {
            status = f.Cancelled ? "cancelled" : "valid",
            issuer = Issuer,
            kind = f.KindName,
            number = f.Number,
            date = f.Date.ToString("yyyy/MM/dd", Inv),
            amount = f.Amount,
            party = MaskName(f.Party),
            partyId = MaskId(f.PartyId),
            branch = f.Branch,
            code = now,
        };
    }

    // Privacy: whoever scans sees enough to match the paper, not the full identity.
    private static string? MaskName(string? n)
    {
        if (string.IsNullOrWhiteSpace(n)) return null;
        var w = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return w.Length <= 1 ? w[0] : $"{w[0]} {string.Join(' ', w.Skip(1).Select(x => x[0] + "***"))}";
    }
    private static string? MaskId(string? id) => string.IsNullOrWhiteSpace(id) || id.Length < 4 ? null : new string('•', id.Length - 4) + id[^4..];
}
