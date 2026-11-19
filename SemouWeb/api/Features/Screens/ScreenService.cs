using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text.Json;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Screens;

public sealed class ChildRow
{
    public Guid BranchGuid { get; set; }
    public decimal? Value { get; set; }
}

public sealed class SaveRequest
{
    public Dictionary<string, JsonElement> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>BranchSet: array of guids. BranchNumbers: array of { branchGuid, value }.</summary>
    public Dictionary<string, JsonElement>? Children { get; set; }
    /// <summary>Required on update (desktop UpdateResaon).</summary>
    public string? Reason { get; set; }
}

public sealed record SaveResult(string Message, string Code, Guid Guid);

/// <summary>Runs any <see cref="ScreenDef"/> with the desktop rules: numbering, validation, procs, children, audit.</summary>
public sealed class ScreenService(Db db, ProcCatalog procs, UserActionLog audit)
{
    /* ============================ Read ============================ */

    public async Task<IEnumerable<IDictionary<string, object?>>> ListAsync(ScreenDef s, Guid? scope, bool? isUse, CancellationToken ct)
    {
        var cols = new List<string> { "T.[Code] AS [Code]", "T.[Guid] AS [Guid]", $"T.[{s.IsUseColumn}] AS [IsUse]" };
        foreach (var col in s.Columns.Where(x => x.Name is not ("Code" or "Guid" or "IsUse")))
            cols.Add(col.Kind == ColumnKind.Lookup ? $"{col.Sql} AS [{col.Name}]" : $"T.[{col.Name}] AS [{col.Name}]");
        if (s.ScopeField != null && !s.Columns.Any(x => x.Name == s.ScopeField)) cols.Add($"T.[{s.ScopeField}] AS [{s.ScopeField}]");

        var where = new List<string>();
        if (s.ScopeField != null) where.Add($"(@scope IS NULL OR T.[{s.ScopeField}] = @scope)");
        where.Add($"(@isUse IS NULL OR T.[{s.IsUseColumn}] = @isUse)");

        var sql = $"SELECT {string.Join(", ", cols)} FROM dbo.[{s.Table}] T WHERE {string.Join(" AND ", where)} ORDER BY TRY_CONVERT(int, T.[Code]), T.[Code]";
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync(new CommandDefinition(sql, new { scope, isUse }, cancellationToken: ct));
        return rows.Cast<IDictionary<string, object?>>();
    }

    public async Task<object> GetAsync(ScreenDef s, Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var row = (IDictionary<string, object?>?)await c.QueryFirstOrDefaultAsync(new CommandDefinition(
            $"SELECT * FROM dbo.[{s.Table}] WHERE Guid = @guid", new { guid }, cancellationToken: ct))
            ?? throw ApiException.NotFound($"{s.Singular} غير موجود");

        var src = new Dictionary<string, object?>(row, StringComparer.OrdinalIgnoreCase);
        var values = new Dictionary<string, object?>();
        foreach (var f in s.Fields) values[f.Name] = src.GetValueOrDefault(f.Name);
        if (src.GetValueOrDefault(s.IsUseColumn) is { } u) values["IsUse"] = u;

        var children = new Dictionary<string, object>();
        foreach (var ch in s.Children)
        {
            children[ch.Name] = ch.Kind == ChildKind.BranchSet
                ? await c.QueryAsync<Guid>(new CommandDefinition(
                    $"SELECT BranchGuid FROM dbo.[{ch.Table}] WHERE [{ch.ParentParam}] = @guid ORDER BY ID", new { guid }, cancellationToken: ct))
                : await c.QueryAsync<ChildRow>(new CommandDefinition(
                    $"SELECT BranchGuid, CAST([{ch.ValueParam}] AS decimal(18,2)) AS Value FROM dbo.[{ch.Table}] WHERE [{ch.ParentParam}] = @guid ORDER BY ID", new { guid }, cancellationToken: ct));
        }
        return new { code = src.GetValueOrDefault("Code")?.ToString(), guid, values, children };
    }

    /* ============================ Write ============================ */

    public async Task<SaveResult> CreateAsync(ScreenDef s, SaveRequest req, CancellationToken ct)
    {
        var values = Validate(s, req.Values, isNew: true);
        var children = ParseChildren(s, req.Children);
        var addParams = await procs.GetAsync(s.AddProc);

        return await db.InTransactionAsync(async (c, tx) =>
        {
            var scope = s.ScopeField != null ? values[s.ScopeField] : null;
            var scopeSql = s.ScopeField != null ? $" WHERE [{s.ScopeField}] = @scope" : "";

            int? nextId = null;
            if (s.IdMode == IdMode.MaxPlusOne)
                nextId = await c.ExecuteScalarAsync<int>($"SELECT ISNULL(MAX(ID), 0) + 1 FROM dbo.[{s.Table}] WITH (UPDLOCK, HOLDLOCK){scopeSql}", new { scope }, tx);

            var p = new DynamicParameters();
            foreach (var prm in addParams)
            {
                var name = prm.Bare;
                if (prm.IsOutput) { p.Add(name, dbType: DbType.Int32, direction: ParameterDirection.Output); continue; }
                if (nextId != null && name.Equals("ID", StringComparison.OrdinalIgnoreCase)) { p.Add(name, nextId); continue; }
                if (nextId != null && name.Equals("Code", StringComparison.OrdinalIgnoreCase)) { p.Add(name, nextId.ToString()); continue; }
                p.Add(name, ToSql(Need(values, name, s.AddProc), prm.SqlType));
            }
            await c.ExecuteAsync(new CommandDefinition("dbo." + s.AddProc, p, tx, commandType: CommandType.StoredProcedure, cancellationToken: ct));

            var outParam = addParams.FirstOrDefault(x => x.IsOutput);
            var code = nextId?.ToString() ?? p.Get<int?>(outParam?.Bare ?? "CodeOut")?.ToString()
                       ?? throw new InvalidOperationException($"{s.AddProc} did not return a code.");

            var guid = await c.ExecuteScalarAsync<Guid>(
                $"SELECT TOP 1 Guid FROM dbo.[{s.Table}] WHERE CAST(Code AS nvarchar(50)) = @code{(s.ScopeField != null ? $" AND [{s.ScopeField}] = @scope" : "")} ORDER BY ID DESC",
                new { code, scope }, tx);

            await SaveChildrenAsync(c, tx, s, guid, children, isNew: true);
            await audit.WriteAsync(c, tx, s.Form, guid, UserActionLog.New, code);
            return new SaveResult("تم حفظ البيانات بنجاح", code, guid);
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<SaveResult> UpdateAsync(ScreenDef s, Guid guid, SaveRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        if (req.Reason.Length > 250) throw new ApiException("سبب الإجراء طويل جداً (الحد الأقصى 250 حرف)");
        var values = Validate(s, req.Values, isNew: false);
        var children = ParseChildren(s, req.Children);
        var updParams = await procs.GetAsync(s.UpdateProc);

        return await db.InTransactionAsync(async (c, tx) =>
        {
            var code = (await c.ExecuteScalarAsync<object?>($"SELECT Code FROM dbo.[{s.Table}] WHERE Guid = @guid", new { guid }, tx))?.ToString()
                       ?? throw ApiException.NotFound($"{s.Singular} غير موجود");

            var p = new DynamicParameters();
            foreach (var prm in updParams)
            {
                if (prm.Bare.Equals("Guid", StringComparison.OrdinalIgnoreCase)) { p.Add(prm.Bare, guid); continue; }
                p.Add(prm.Bare, ToSql(Need(values, prm.Bare, s.UpdateProc), prm.SqlType));
            }
            await c.ExecuteAsync(new CommandDefinition("dbo." + s.UpdateProc, p, tx, commandType: CommandType.StoredProcedure, cancellationToken: ct));

            await SaveChildrenAsync(c, tx, s, guid, children, isNew: false);
            await audit.WriteAsync(c, tx, s.Form, guid, UserActionLog.Update, code, req.Reason.Trim());
            return new SaveResult("تم تعديل البيانات بنجاح", code, guid);
        }, ct: ct);
    }

    /* ============================ Children ============================ */

    private static Dictionary<string, List<ChildRow>> ParseChildren(ScreenDef s, Dictionary<string, JsonElement>? input)
    {
        var result = new Dictionary<string, List<ChildRow>>();
        foreach (var ch in s.Children)
        {
            var rows = new List<ChildRow>();
            if (input != null && input.TryGetValue(ch.Name, out var el) && el.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in el.EnumerateArray())
                {
                    if (ch.Kind == ChildKind.BranchSet && item.ValueKind == JsonValueKind.String && Guid.TryParse(item.GetString(), out var g))
                        rows.Add(new ChildRow { BranchGuid = g });
                    else if (ch.Kind == ChildKind.BranchNumbers && item.ValueKind == JsonValueKind.Object
                             && item.TryGetProperty("branchGuid", out var bg) && Guid.TryParse(bg.GetString(), out var g2))
                    {
                        var v = item.TryGetProperty("value", out var ve) && ve.ValueKind == JsonValueKind.Number ? ve.GetDecimal() : 0;
                        if (v < 0) throw new ApiException($"{ch.ValueLabel} لا يمكن أن يكون بالسالب");
                        rows.Add(new ChildRow { BranchGuid = g2, Value = v });
                    }
                }
            }
            rows = rows.DistinctBy(r => r.BranchGuid).ToList();
            if (ch.Required != null && rows.Count == 0) throw new ApiException(ch.Required);
            result[ch.Name] = rows;
        }
        return result;
    }

    private async Task SaveChildrenAsync(DbConnection c, DbTransaction tx, ScreenDef s, Guid parent, Dictionary<string, List<ChildRow>> children, bool isNew)
    {
        foreach (var ch in s.Children)
        {
            if (!isNew)
                await c.ExecuteAsync("dbo." + ch.DeleteProc, new DynamicParameters(new Dictionary<string, object> { [ch.ParentParam] = parent }), tx, commandType: CommandType.StoredProcedure);

            var addParams = await procs.GetAsync(ch.AddProc);
            foreach (var row in children[ch.Name])
            {
                var p = new DynamicParameters();
                foreach (var prm in addParams)
                {
                    if (prm.Bare.Equals(ch.ParentParam, StringComparison.OrdinalIgnoreCase)) p.Add(prm.Bare, parent);
                    else if (prm.Bare.Equals("BranchGuid", StringComparison.OrdinalIgnoreCase)) p.Add(prm.Bare, row.BranchGuid);
                    else if (ch.ValueParam != null && prm.Bare.Equals(ch.ValueParam, StringComparison.OrdinalIgnoreCase)) p.Add(prm.Bare, ToSql(row.Value ?? 0, prm.SqlType));
                    else throw new InvalidOperationException($"{ch.AddProc}: unmapped parameter {prm.Name}");
                }
                await c.ExecuteAsync("dbo." + ch.AddProc, p, tx, commandType: CommandType.StoredProcedure);
            }
        }
    }

    /* ============================ Validation & conversion ============================ */

    /// <summary>VALIDATE_DATA: checks in field order; the first failing message is the one shown (like the desktop).</summary>
    private static Dictionary<string, object?> Validate(ScreenDef s, Dictionary<string, JsonElement> input, bool isNew)
    {
        var src = new Dictionary<string, JsonElement>(input, StringComparer.OrdinalIgnoreCase);
        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();

        foreach (var f in s.Fields)
        {
            src.TryGetValue(f.Name, out var el);
            var empty = el.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                        || (el.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(el.GetString()));
            object? v;
            switch (f.Type)
            {
                case FieldType.Bool:
                    v = el.ValueKind == JsonValueKind.True || (el.ValueKind != JsonValueKind.False && f.Default is true);
                    break;
                case FieldType.Int:
                case FieldType.Decimal:
                    decimal n = 0;
                    if (!empty && !(el.ValueKind == JsonValueKind.Number ? el.TryGetDecimal(out n)
                            : decimal.TryParse(el.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out n)))
                    { errors.Add($"{f.Label}: قيمة غير صحيحة"); n = 0; }
                    if (f.Required != null && (empty || (f.NonZero && n == 0))) errors.Add(f.Required);
                    else if (n < 0) errors.Add($"{f.Label} لا يمكن أن يكون بالسالب");
                    v = f.Type == FieldType.Int ? (object)decimal.ToInt32(decimal.Truncate(n)) : n;
                    break;
                case FieldType.Select:
                    int idx = -1;
                    if (!empty && el.ValueKind == JsonValueKind.Number) el.TryGetInt32(out idx);
                    if (idx < 0 && f.Default is int d) idx = d;
                    if (idx < 0 || idx >= (f.Options?.Length ?? 0)) { if (f.Required != null) errors.Add(f.Required); else idx = -1; }
                    v = idx; // desktop stores SelectedIndex (-1 when nothing is chosen)
                    break;
                case FieldType.Lookup:
                    Guid? g = !empty && el.ValueKind == JsonValueKind.String && Guid.TryParse(el.GetString(), out var gg) ? gg : null;
                    if (g == null && f.Required != null && !(f.LockOnEdit && !isNew)) errors.Add(f.Required);
                    v = g;
                    break;
                default: // Text, TextArea, Combo
                    var str = empty ? "" : (el.ValueKind == JsonValueKind.String ? el.GetString()! : el.ToString()).Trim();
                    if (str.Length == 0 && f.Default is string ds) str = ds;
                    if (f.Required != null && str.Length == 0) errors.Add(f.Required);
                    else if (f.MaxLength is { } max && str.Length > max) errors.Add($"{f.Label}: الحد الأقصى {max} حرف");
                    v = str;
                    break;
            }
            values[f.Name] = v;
        }
        if (errors.Count > 0) throw new ApiException(errors[0]);
        return values;
    }

    private static object? Need(Dictionary<string, object?> values, string param, string proc) =>
        values.TryGetValue(param, out var v) ? v : throw new InvalidOperationException($"{proc}: no field for parameter @{param}");

    /// <summary>Converts to the exact SQL type the proc declares (e.g. a checkbox into a bigint column).</summary>
    private static object? ToSql(object? v, string sqlType) => (v, sqlType.ToLowerInvariant()) switch
    {
        (null, _) => DBNull.Value,
        (bool b, "bigint") => b ? 1L : 0L,
        (bool b, "int") => b ? 1 : 0,
        (decimal d, "float") => (double)d,
        (decimal d, "int") => decimal.ToInt32(decimal.Truncate(d)),
        (decimal d, "bigint") => decimal.ToInt64(decimal.Truncate(d)),
        (int i, "float") => (double)i,
        _ => v,
    };
}
