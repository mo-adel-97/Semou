/* =====================================================================
   ترقيم فواتير النهارده 2026/10/11 اللي اتعملت قبل تشغيل 01_TaxInvoiceNo.sql
   5173 -> 01 ، 5174 -> 02 ، 5175 -> 03
   + تعديل رقم الفاتورة في بيانات الإرسال للهيئة (BillByJson) للي لسه متبعتتش
   شغّله مرة واحدة بس. لو في حاجة اتغيرت (فاتورة جديدة اتعملت أو فاتورة اتبعتت) هيوقف من غير ما يغير حاجة.
   ===================================================================== */
USE [Semou]
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRAN;

IF EXISTS (SELECT 1 FROM dbo.Bill WHERE TaxNo IS NOT NULL)
   OR EXISTS (SELECT 1 FROM dbo.BillByJson j JOIN dbo.Bill b ON b.Guid = j.BillGuid
              WHERE b.ID IN (5173, 5174, 5175) AND j.SendCheck = 1)
BEGIN
    ROLLBACK;
    RAISERROR(N'في فاتورة جديدة اتعملت أو فاتورة اتبعتت للهيئة - السكريبت وقف ومغيرش حاجة', 16, 1);
    RETURN;
END

UPDATE dbo.Bill SET TaxNo = NEXT VALUE FOR dbo.TaxInvoiceNoSeq WHERE ID = 5173;
UPDATE dbo.Bill SET TaxNo = NEXT VALUE FOR dbo.TaxInvoiceNoSeq WHERE ID = 5174;
UPDATE dbo.Bill SET TaxNo = NEXT VALUE FOR dbo.TaxInvoiceNoSeq WHERE ID = 5175;

UPDATE j
   SET JsonCintent = JSON_MODIFY(j.JsonCintent, '$.id', RIGHT('0' + CAST(b.TaxNo AS varchar(10)), 2))
  FROM dbo.BillByJson j
  JOIN dbo.Bill b ON b.Guid = j.BillGuid
 WHERE b.ID IN (5173, 5174, 5175)
   AND ISNULL(j.SendCheck, 0) = 0
   AND ISJSON(j.JsonCintent) = 1;

COMMIT;

-- مراجعة: المفروض 01 ، 02 ، 03 والعداد بقى 3
SELECT b.ID, b.TaxNo, JSON_VALUE(j.JsonCintent, '$.id') AS json_id, j.SendCheck
  FROM dbo.Bill b LEFT JOIN dbo.BillByJson j ON j.BillGuid = b.Guid
 WHERE b.ID >= 5173 ORDER BY b.ID;
SELECT current_value FROM sys.sequences WHERE name = 'TaxInvoiceNoSeq';
GO
