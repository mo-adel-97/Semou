CREATE proc [dbo].[AddBillByJson]
@BillCode as nvarchar(50),
@BillGuid as uniqueidentifier,
@JsonCintent as nvarchar(max),
@ZacatType as int,
@BillRef as nvarchar(50) = NULL,
@RefundNotes as nvarchar(250) = NULL
as
begin
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
