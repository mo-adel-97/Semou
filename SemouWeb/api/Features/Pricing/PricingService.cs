using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Pricing;

/*
 * ضبط الأسعار والدفعات المقدمة (web only, db/2026-11-28_price_plans.sql).
 * A package (WebPricePlan) is a way of paying: cash, or a down payment then N monthly instalments. Every diploma gets
 * its own price and down payment in every package (WebPlanPrice). The registration wizard offers the packages of the
 * chosen diploma; the server re-reads the price (RegistrationService.PlanAsync), so the browser's numbers are never trusted.
 */

public sealed class PlanInput
{
    public string? Name { get; set; }
    public int Months { get; set; }
    public string? Description { get; set; }
    public int Sort { get; set; }
    public bool IsUse { get; set; } = true;
    public string? Reason { get; set; }
}

public sealed class PriceCell
{
    public Guid PlanGuid { get; set; }
    public Guid DiplomGuid { get; set; }
    public decimal Price { get; set; }
    public decimal DownPayment { get; set; }
    public bool IsUse { get; set; } = true;
}

public sealed class PricesInput
{
    public List<PriceCell> Items { get; set; } = [];
    public string? Reason { get; set; }
}

/// <summary>The package a diploma is sold with, as the registration needs it.</summary>
public sealed record PlanOffer(Guid PlanGuid, string Name, int Months, string? Description, decimal Price, decimal DownPayment, decimal Installment);

public sealed class PricingService(Db db, UserActionLog audit)
{
    public const string Form = WebForms.PricePlans;

    public static decimal InstallmentOf(decimal price, decimal down, int months) =>
        months > 0 ? Math.Round((price - down) / months, 2, MidpointRounding.AwayFromZero) : 0;

    public async Task<object> GetAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var plans = await c.QueryAsync(new CommandDefinition("""
            SELECT P.Guid, P.Code, P.Name, P.Months, P.Description, P.Sort, P.IsUse, P.UpdatedAt,
                   (SELECT COUNT(*) FROM dbo.WebRegPlan R WHERE R.PlanGuid = P.Guid) AS Used
            FROM dbo.WebPricePlan P ORDER BY P.Sort, P.Code
            """, cancellationToken: ct));
        // the diplomas sold on a registration form (StudyType 0 = دبلوم, DiplomType 0 = item, not a fee)
        var diploms = await c.QueryAsync(new CommandDefinition("""
            SELECT D.Guid, D.Code, D.DiplomName AS Name, D.EnglishName, CAST(ISNULL(D.TaxValue,0) AS decimal(9,2)) AS TaxRate, CAST(ISNULL(D.IsUse,0) AS bit) AS IsUse
            FROM dbo.Diplom D WHERE D.StudyType = 0 AND D.DiplomType = 0
            ORDER BY CAST(ISNULL(D.IsUse,0) AS int) DESC, TRY_CONVERT(int, D.Code)
            """, cancellationToken: ct));
        var prices = await c.QueryAsync(new CommandDefinition("""
            SELECT X.PlanGuid, X.DiplomGuid, X.Price, X.DownPayment, X.IsUse, X.UpdatedAt, U.FullName AS UpdatedBy
            FROM dbo.WebPlanPrice X LEFT JOIN dbo.User_Info U ON U.Guid = X.UpdatedBy
            """, cancellationToken: ct));
        return new { plans, diploms, prices };
    }

    private static void Check(PlanInput i)
    {
        i.Name = i.Name?.Trim();
        if (string.IsNullOrEmpty(i.Name)) throw new ApiException("يرجى إدخال اسم الباقة");
        if (i.Name.Length > 100) throw new ApiException("اسم الباقة طويل جداً (100 حرف كحد أقصى)");
        if (i.Months is < 0 or > 120) throw new ApiException("عدد الشهور بين 0 (كاش) و120 شهراً");
        i.Description = string.IsNullOrWhiteSpace(i.Description) ? null : i.Description.Trim();
        if (i.Description?.Length > 250) throw new ApiException("وصف الباقة طويل جداً (250 حرفاً كحد أقصى)");
    }

    private static async Task UniqueAsync(DbConnection c, DbTransaction tx, string name, Guid? except)
    {
        if (await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.WebPricePlan WHERE Name = @name AND (@except IS NULL OR Guid <> @except)", new { name, except }, tx) > 0)
            throw new ApiException("توجد باقة بنفس الاسم");
    }

    public Task<object> AddPlanAsync(PlanInput i, CancellationToken ct)
    {
        Check(i);
        return db.InTransactionAsync<object>(async (c, tx) =>
        {
            await UniqueAsync(c, tx, i.Name!, null);
            var code = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Code),0) + 1 FROM dbo.WebPricePlan WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            var guid = Guid.NewGuid();
            var sort = i.Sort > 0 ? i.Sort : await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Sort),0) + 1 FROM dbo.WebPricePlan", transaction: tx);
            await c.ExecuteAsync("INSERT dbo.WebPricePlan (Guid, Code, Name, Months, Description, Sort, IsUse) VALUES (@guid, @code, @Name, @Months, @Description, @sort, @IsUse)",
                new { guid, code, i.Name, i.Months, i.Description, sort, i.IsUse }, tx);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.New, code.ToString());
            return new { message = "تمت إضافة الباقة", guid };
        }, ct: ct);
    }

    public Task<object> UpdatePlanAsync(Guid guid, PlanInput i, CancellationToken ct)
    {
        Check(i);
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
        return db.InTransactionAsync<object>(async (c, tx) =>
        {
            var code = await c.ExecuteScalarAsync<int?>("SELECT Code FROM dbo.WebPricePlan WITH (UPDLOCK) WHERE Guid = @guid", new { guid }, tx)
                       ?? throw ApiException.NotFound("الباقة غير موجودة");
            await UniqueAsync(c, tx, i.Name!, guid);
            await c.ExecuteAsync("UPDATE dbo.WebPricePlan SET Name = @Name, Months = @Months, Description = @Description, Sort = @Sort, IsUse = @IsUse, UpdatedAt = GETDATE() WHERE Guid = @guid",
                new { guid, i.Name, i.Months, i.Description, i.Sort, i.IsUse }, tx);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.Update, code.ToString(), i.Reason.Trim());
            return new { message = "تم تعديل الباقة" };
        }, ct: ct);
    }

    public Task<object> SavePricesAsync(PricesInput i, Guid user, CancellationToken ct)
    {
        if (i.Items.Count == 0) throw new ApiException("لا توجد تعديلات للحفظ");
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
        return db.InTransactionAsync<object>(async (c, tx) =>
        {
            var plans = (await c.QueryAsync<(Guid Guid, string Name)>("SELECT Guid, Name FROM dbo.WebPricePlan", transaction: tx)).ToDictionary(x => x.Guid, x => x.Name);
            var diploms = (await c.QueryAsync<(Guid Guid, string Name)>("SELECT Guid, DiplomName FROM dbo.Diplom WHERE StudyType = 0 AND DiplomType = 0", transaction: tx)).ToDictionary(x => x.Guid, x => x.Name);
            foreach (var x in i.Items.DistinctBy(x => (x.PlanGuid, x.DiplomGuid)))
            {
                if (!plans.TryGetValue(x.PlanGuid, out var plan)) throw new ApiException("إحدى الباقات غير موجودة");
                if (!diploms.TryGetValue(x.DiplomGuid, out var dip)) throw new ApiException("أحد الدبلومات غير موجود");
                if (x.Price < 0 || x.DownPayment < 0) throw new ApiException($"لا يمكن إدخال قيمة بالسالب ({dip} • {plan})");
                if (x.IsUse && x.Price == 0) throw new ApiException($"يرجى إدخال سعر {dip} في باقة {plan}، أو إيقاف الباقة لهذا الدبلوم");
                if (x.DownPayment > x.Price) throw new ApiException($"الدفعة المقدمة أكبر من السعر ({dip} • {plan})");
                await c.ExecuteAsync("""
                    UPDATE dbo.WebPlanPrice SET Price = @Price, DownPayment = @DownPayment, IsUse = @IsUse, UpdatedAt = GETDATE(), UpdatedBy = @user
                    WHERE PlanGuid = @PlanGuid AND DiplomGuid = @DiplomGuid;
                    IF @@ROWCOUNT = 0
                        INSERT dbo.WebPlanPrice (PlanGuid, DiplomGuid, Price, DownPayment, IsUse, UpdatedBy) VALUES (@PlanGuid, @DiplomGuid, @Price, @DownPayment, @IsUse, @user);
                    """, new { x.PlanGuid, x.DiplomGuid, x.Price, x.DownPayment, x.IsUse, user }, tx);
                await audit.WriteAsync(c, tx, Form, x.DiplomGuid, UserActionLog.Update, $"{plan}: {x.Price:0.##} / {x.DownPayment:0.##}", i.Reason.Trim());
            }
            return new { message = "تم حفظ الأسعار والدفعات المقدمة" };
        }, ct: ct);
    }

    /// <summary>The active packages offered for these diplomas (registration wizard + server check).</summary>
    public static async Task<Dictionary<Guid, List<PlanOffer>>> OffersAsync(DbConnection c, DbTransaction? tx, IEnumerable<Guid> diploms)
    {
        var list = diploms.Distinct().ToArray();
        if (list.Length == 0 || await c.ExecuteScalarAsync<int>("SELECT CASE WHEN OBJECT_ID('dbo.WebPlanPrice') IS NULL THEN 0 ELSE 1 END", transaction: tx) == 0) return [];
        var rows = await c.QueryAsync<(Guid DiplomGuid, Guid PlanGuid, string Name, int Months, string? Description, decimal Price, decimal DownPayment)>("""
            SELECT X.DiplomGuid, P.Guid, P.Name, P.Months, P.Description, X.Price, X.DownPayment
            FROM dbo.WebPlanPrice X JOIN dbo.WebPricePlan P ON P.Guid = X.PlanGuid
            WHERE X.IsUse = 1 AND P.IsUse = 1 AND X.Price > 0 AND X.DiplomGuid IN @list
            ORDER BY P.Sort, P.Code
            """, new { list }, tx);
        return rows.GroupBy(r => r.DiplomGuid).ToDictionary(g => g.Key,
            g => g.Select(r => new PlanOffer(r.PlanGuid, r.Name, r.Months, r.Description, r.Price, r.DownPayment, InstallmentOf(r.Price, r.DownPayment, r.Months))).ToList());
    }
}
