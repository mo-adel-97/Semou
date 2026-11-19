CREATE proc [dbo].[PrintBill]
@BillGuid as uniqueidentifier
as
begin
SELECT        dbo.Bill.Code, dbo.Bill.BillDate, STUFF(RIGHT(CONVERT(VarChar(19),dbo.Bill.BillDate , 0), 7), 6, 0, ' ') as TimeOnly, dbo.User_Info.FullName, dbo.Student.StudentName, dbo.Student.StudentTel,
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

