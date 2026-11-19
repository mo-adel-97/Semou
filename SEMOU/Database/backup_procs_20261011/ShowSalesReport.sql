CREATE proc [dbo].[ShowSalesReport]
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
SELECT dbo.Bill.BillDate, dbo.Bill.Code, dbo.RegSetting.DocName, dbo.Student.StudentName, dbo.Student.StudentTel,
       dbo.Student.NationalId, dbo.Bill.Notes, dbo.Bill.BillTotal, dbo.Bill.BillTax, dbo.Bill.BillSubTotal, 
       dbo.Branch.BrEName, dbo.User_Info.FullName
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
			 (dbo.Bill.Code =  @BillCode  OR @BillCode IS NULL) AND
			 (dbo.RegSetting.InvoiceType = @ChkStartPay OR dbo.RegSetting.InvoiceType = @ChkMonPay OR dbo.RegSetting.InvoiceType = @ChkFeesPay )
ORDER BY TRY_CAST(dbo.Bill.Code AS INT)
end

