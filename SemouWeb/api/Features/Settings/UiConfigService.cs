using System.Data;
using System.Text.Json;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Settings;

/// <summary>
/// الإعدادات العامة: the sidebar structure ("menu") and the interface texts ("texts"), stored as versioned JSON in
/// dbo.WebUiConfig. The current version of a section is its latest row; every save adds a row, so any earlier
/// version can be restored. The content is validated as JSON only: the screen owns its shape.
/// </summary>
public sealed class UiConfigService(Db db, CurrentUser me)
{
    public static readonly string[] Sections = ["menu", "texts"];
    private const int MaxChars = 1_000_000;

    public sealed record SaveInput(JsonElement Data, int? BaseVersion, string? Note);

    private static void Check(string section)
    {
        if (!Sections.Contains(section)) throw ApiException.NotFound("قسم الإعدادات غير موجود");
    }

    /// <summary>The current settings of every section (read by every logged-in user to draw the interface).</summary>
    public async Task<object> GetAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        if (await c.ExecuteScalarAsync<int?>("SELECT OBJECT_ID('dbo.WebUiConfig')") is null)
            return new { menu = (object?)null, texts = (object?)null, versions = new Dictionary<string, int>() };
        var rows = (await c.QueryAsync<(string Section, int ID, string Json)>(new CommandDefinition("""
            SELECT X.Section, X.ID, X.Json FROM dbo.WebUiConfig X
            WHERE X.ID = (SELECT MAX(Y.ID) FROM dbo.WebUiConfig Y WHERE Y.Section = X.Section)
            """, cancellationToken: ct))).ToList();
        JsonElement? Of(string s) => rows.FirstOrDefault(r => r.Section == s) is { Json: { } j } ? JsonDocument.Parse(j).RootElement : null;
        return new { menu = Of("menu"), texts = Of("texts"), versions = rows.ToDictionary(r => r.Section, r => r.ID) };
    }

    public async Task<object> SaveAsync(string section, SaveInput i, CancellationToken ct)
    {
        Check(section);
        if (i.Data.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array)) throw new ApiException("بيانات الإعدادات غير صحيحة");
        var json = i.Data.GetRawText();
        if (json.Length > MaxChars) throw new ApiException("حجم الإعدادات أكبر من المسموح");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var latest = await c.ExecuteScalarAsync<int?>("SELECT MAX(ID) FROM dbo.WebUiConfig WITH (UPDLOCK, HOLDLOCK) WHERE Section = @section", new { section }, tx);
            if (latest != i.BaseVersion)
                throw new ApiException("عدّل مستخدم آخر هذه الإعدادات بعد أن فتحتَها. حدّث الصفحة لرؤية آخر نسخة ثم أعد تعديلك.", 409);
            var id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO dbo.WebUiConfig (Section, Json, UserGuid, SavedAt, Note) OUTPUT inserted.ID VALUES (@section, @json, @u, @now, @note)
                """, new { section, json, u = me.UserGuid, now = Clock.Now, note = Cut(i.Note) }, tx);
            return (object)new { message = "تم حفظ الإعدادات وتطبيقها على جميع المستخدمين", version = id };
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<IEnumerable<object>> HistoryAsync(string section, CancellationToken ct)
    {
        Check(section);
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync<(int ID, DateTime SavedAt, string? UserName, string? Note, int Size)>(new CommandDefinition("""
            SELECT TOP 40 X.ID, X.SavedAt, U.FullName, X.Note, LEN(X.Json) FROM dbo.WebUiConfig X
            LEFT JOIN dbo.User_Info U ON U.Guid = X.UserGuid WHERE X.Section = @section ORDER BY X.ID DESC
            """, new { section }, cancellationToken: ct));
        return rows.Select(r => (object)new { id = r.ID, savedAt = r.SavedAt, user = r.UserName, note = r.Note, size = r.Size });
    }

    /// <summary>A saved version, to preview or restore (restoring = saving it again as the newest version).</summary>
    public async Task<object> VersionAsync(string section, int id, CancellationToken ct)
    {
        Check(section);
        await using var c = await db.OpenAsync(ct);
        var json = await c.ExecuteScalarAsync<string?>(new CommandDefinition("SELECT Json FROM dbo.WebUiConfig WHERE ID = @id AND Section = @section", new { id, section }, cancellationToken: ct))
                   ?? throw ApiException.NotFound("النسخة غير موجودة");
        return new { id, data = JsonDocument.Parse(json).RootElement };
    }

    private static string? Cut(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim() is var t && t.Length > 200 ? t[..200] : s.Trim();
}
