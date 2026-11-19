/* =====================================================================
   تقرير المبيعات + التقرير الضريبي: عمود "رقم الفاتورة" = رقم الفاتورة الضريبية
   (01 ، 02 ...) للفواتير من النهارده. الفواتير القديمة برقمها القديم زي ما هي.
   - البحث برقم الفاتورة يقبل الرقم الضريبي (4 أو 04) أو الرقم الداخلي.
   - عمود جديد في الآخر InternalCode = الرقم الداخلي (للنسخة الجديدة - زرار عرض).
     النسخة القديمة بتتجاهله.
   النسخة الأصلية في: Database\backup_procs_20261011
   ===================================================================== */
USE [Semou]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[ShowSalesReport]
@FDate as date,
@TDate as date,
@BranchGuid as uniqueidentifier = NULL,
@UserGuid as uniqueidentifier = NULL,
@DocGuid as uniqueidentifier = NULL,
@NationalId as nvarchar(50) = NULL,
@BillCode as nvarchar(50) = NULL,
@IsUse as bit,
@ChkStartPay as int = NULL,
@ChkMonPay as int = NULL,
@ChkFeesPay as int = NULL
as
begin
SELECT dbo.Bill.BillDate,
       ISNULL(CASE WHEN dbo.Bill.TaxNo < 10 THEN N'0' ELSE N'' END + CAST(dbo.Bill.TaxNo AS nvarchar(10)), dbo.Bill.Code) AS Code,
       dbo.RegSetting.DocName, dbo.Student.StudentName, dbo.Student.StudentTel,
       dbo.Student.NationalId, dbo.Bill.Notes, dbo.Bill.BillTotal, dbo.Bill.BillTax, dbo.Bill.BillSubTotal,
       dbo.Branch.BrEName, dbo.User_Info.FullName,
       dbo.Bill.Code AS InternalCode
FROM            dbo.Bill WITH (NOLOCK) INNER JOIN
                         dbo.RegSetting ON dbo.Bill.DocGuid = dbo.RegSetting.Guid INNER JOIN
                         dbo.Student ON dbo.Bill.AccountGuid = dbo.Student.AccountGuid INNER JOIN
                         dbo.User_Info ON dbo.Bill.UserGuid = dbo.User_Info.Guid INNER JOIN
                         dbo.Branch ON dbo.Bill.BranchGuid = dbo.Branch.Guid
WHERE        (CONVERT(DATE, dbo.Bill.BillDate) BETWEEN CONVERT(DATE, @FDate, 102) AND CONVERT(DATE, @TDate, 102)) AND
             (dbo.Bill.BranchGuid = @BranchGuid OR @BranchGuid IS NULL) AND
			 (dbo.Bill.UserGuid = @UserGuid OR @UserGuid IS NULL) AND
			 (dbo.Bill.DocGuid = @DocGuid OR @DocGuid IS NULL) AND
             (dbo.Student.StudentName like '%' + @NationalId + '%'  OR @NationalId IS NULL) AND
			 (dbo.Bill.IsUse = @IsUse ) AND
			 (dbo.Bill.Code = @BillCode OR dbo.Bill.TaxNo = TRY_CAST(@BillCode AS int) OR @BillCode IS NULL) AND
			 (dbo.RegSetting.InvoiceType = @ChkStartPay OR dbo.RegSetting.InvoiceType = @ChkMonPay OR dbo.RegSetting.InvoiceType = @ChkFeesPay )
ORDER BY TRY_CAST(dbo.Bill.Code AS INT)
end
GO
ALTER proc [dbo].[ShowTaxReport]
@FDate as date,
@TDate as date,
@BranchGuid as uniqueidentifier = NULL,
@NationalId as nvarchar(50) = NULL,
@BillCode as nvarchar(50) = NULL,
@IsUse as bit
as
begin
SELECT  dbo.Bill.BillDate,
        ISNULL(CASE WHEN dbo.Bill.TaxNo < 10 THEN N'0' ELSE N'' END + CAST(dbo.Bill.TaxNo AS nvarchar(10)), dbo.Bill.Code) AS Code,
        dbo.RegSetting.DocName, dbo.Student.StudentName,
        dbo.Student.StudentTel, dbo.Student.NationalId, dbo.Bill.Notes, dbo.RegDocDiplom.Cost,
		dbo.RegDocDiplom.Tax,dbo.RegDocDiplom.SubTotal, dbo.Branch.BrEName, dbo.User_Info.FullName,
		dbo.Diplom.DiplomName, (convert(nvarchar,dbo.Diplom.CountNum) + '_'+ dbo.Diplom.CountText) as TIMES,
		dbo.Bill.Code AS InternalCode
FROM            dbo.Bill WITH (NOLOCK) INNER JOIN
                         dbo.RegSetting ON dbo.Bill.DocGuid = dbo.RegSetting.Guid INNER JOIN
                         dbo.Student ON dbo.Bill.AccountGuid = dbo.Student.AccountGuid INNER JOIN
                         dbo.User_Info ON dbo.Bill.UserGuid = dbo.User_Info.Guid INNER JOIN
                         dbo.Branch ON dbo.Bill.BranchGuid = dbo.Branch.Guid INNER JOIN
                         dbo.RegDocDiplom ON dbo.Bill.Guid = dbo.RegDocDiplom.RegDocGuid INNER JOIN
                         dbo.Diplom ON dbo.RegDocDiplom.DiplomGuid = dbo.Diplom.Guid
WHERE        (CONVERT(DATE, dbo.Bill.BillDate) BETWEEN CONVERT(DATE, @FDate, 102) AND CONVERT(DATE, @TDate, 102)) AND
             (dbo.Bill.BranchGuid = @BranchGuid OR @BranchGuid IS NULL) AND
             (dbo.Student.StudentName like '%' + @NationalId + '%'  OR @NationalId IS NULL) AND
			 (dbo.Bill.Code = @BillCode OR dbo.Bill.TaxNo = TRY_CAST(@BillCode AS int) OR @BillCode IS NULL)
ORDER BY TRY_CAST(dbo.Bill.Code AS INT)
end
GO
