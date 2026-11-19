using System.Data;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Branches;

/// <summary>Business rules of the desktop EncodingBranch screen.</summary>
public sealed class BranchService(Db db, BranchRepository repo, UserActionLog audit)
{
    public const string Form = "addbranch";

    public Task<IEnumerable<BranchListItem>> SearchAsync(string? q, bool? isUse, bool? isRegOn, CancellationToken ct)
        => repo.SearchAsync(string.IsNullOrWhiteSpace(q) ? null : q.Trim(), isUse, isRegOn, ct);

    public async Task<BranchDetails> GetAsync(string code, CancellationToken ct)
        => await repo.GetByCodeAsync(code, ct) ?? throw ApiException.NotFound("الفرع غير موجود");

    /// <summary>FORSAVE: AddBranch (ID = Code = max+1), then the audit row "جديد", all in one transaction.</summary>
    public Task<BranchSaved> CreateAsync(BranchInput input, CancellationToken ct) =>
        db.InTransactionAsync(async (c, tx) =>
        {
            var id = await repo.NextIdAsync(c, tx);
            var code = id.ToString();
            await repo.InsertAsync(c, tx, id, code, input);
            var guid = await repo.GetGuidAsync(c, tx, code);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.New, code);
            return new BranchSaved("تم حفظ البيانات بنجاح", code, guid);
        }, IsolationLevel.Serializable, ct);

    /// <summary>FORUPDATE: UpdateBranch, then the audit row "تعديل" with the reason.</summary>
    public Task<BranchSaved> UpdateAsync(Guid guid, UpdateBranchRequest req, CancellationToken ct) =>
        db.InTransactionAsync(async (c, tx) =>
        {
            var code = await repo.GetCodeAsync(c, tx, guid) ?? throw ApiException.NotFound("الفرع غير موجود");
            await repo.UpdateAsync(c, tx, guid, req.Branch);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.Update, code, req.Reason.Trim());
            return new BranchSaved("تم تعديل البيانات بنجاح", code, guid);
        }, ct: ct);
}
