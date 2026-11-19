/* =====================================================================
   حماية للنسخ القديمة من البرنامج (لحد ما كل الأجهزة تتحدث)
   - PrintBill     : عمود Code يرجع رقم الفاتورة الضريبية (01 ، 02 ...) لو موجود
                     فالنسخة القديمة تطبع الرقم الجديد. الفواتير القديمة (من غير TaxNo) زي ما هي.
   - AddBillByJson : رقم الفاتورة اللي بيتبعت للهيئة (id في JSON) = الرقم الضريبي
                     مهما كانت نسخة البرنامج اللي عملت الفاتورة.
   النسخة الأصلية من الإجراءين في: Database\backup_procs_20261011
   ===================================================================== */
USE [Semou]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[PrintBill]
@BillGuid as uniqueidentifier
as
begin
SELECT        ISNULL(CASE WHEN dbo.Bill.TaxNo < 10 THEN N'0' ELSE N'' END + CAST(dbo.Bill.TaxNo AS nvarchar(10)), dbo.Bill.Code) AS Code,
              dbo.Bill.BillDate, STUFF(RIGHT(CONVERT(VarChar(19),dbo.Bill.BillDate , 0), 7), 6, 0, ' ') as TimeOnly, dbo.User_Info.FullName, dbo.Student.StudentName, dbo.Student.StudentTel,
              dbo.Student.NationalId, dbo.Diplom.Code AS ItemCode, dbo.Diplom.DiplomName, dbo.RegDocDiplom.Cost,
              dbo.RegDocDiplom.Tax, dbo.RegDocDiplom.SubTotal, dbo.RegDocDiplom.Qty, dbo.RegDocDiplom.Unit_,dbo.Diplom.TaxValue,
			  dbo.Bill.BillTotal, dbo.Bill.BillTax, dbo.Bill.BillSubTotal
FROM            dbo.Bill INNER JOIN
                         dbo.Student ON dbo.Bill.AccountGuid = dbo.Student.AccountGuid INNER JOIN
                         dbo.User_Info ON dbo.Bill.UserGuid = dbo.User_Info.Guid INNER JOIN
                         dbo.RegDocDiplom ON dbo.Bill.Guid = dbo.RegDocDiplom.RegDocGuid INNER JOIN
                         dbo.Diplom ON dbo.RegDocDiplom.DiplomGuid = dbo.Diplom.Guid
WHERE        (dbo.Bill.Guid = @BillGuid) and  (dbo.RegDocDiplom.Type_ = 1)
end
GO
ALTER proc [dbo].[AddBillByJson]
@BillCode as nvarchar(50),
@BillGuid as uniqueidentifier,
@JsonCintent as nvarchar(max),
@ZacatType as int,
@BillRef as nvarchar(50) = NULL,
@RefundNotes as nvarchar(250) = NULL
as
begin
-- رقم الفاتورة للهيئة = الرقم الضريبي (للفواتير اللي ليها TaxNo بس)
DECLARE @TaxNo int = (SELECT TaxNo FROM dbo.Bill WHERE Guid = @BillGuid)
IF @ZacatType = 388 AND @TaxNo IS NOT NULL AND ISJSON(@JsonCintent) = 1
    SET @JsonCintent = JSON_MODIFY(@JsonCintent, '$.id', CASE WHEN @TaxNo < 10 THEN N'0' ELSE N'' END + CAST(@TaxNo AS nvarchar(10)))

INSERT INTO [dbo].[BillByJson]
           ([BillCode]
           ,[BillGuid]
           ,[JsonCintent]
           ,[CeeatedAt]
           ,[SendCheck]
		   ,[ZacatType]
		   ,[BillRef]
		   ,[RefundNotes])
     VALUES
           (@BillCode,@BillGuid,@JsonCintent,GETDATE (),0,@ZacatType,@BillRef,@RefundNotes)
end
GO
