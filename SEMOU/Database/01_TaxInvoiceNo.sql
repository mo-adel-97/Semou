/* =====================================================================
   ترقيم الفواتير الضريبية الجديد (الرقم الضريبي الجديد 310082966400003)
   ---------------------------------------------------------------------
   - Bill.Code / Bill.ID  : الرقم الداخلي (زي ما هو - مربوط بالقيود وسندات القبض)
   - Bill.TaxNo          : رقم الفاتورة الضريبية الجديد 01 ، 02 ، 03 ...
   الفواتير القديمة تفضل TaxNo = NULL وتطبع برقمها القديم.
   كل فاتورة جديدة بعد تشغيل السكريبت تاخد رقم من 1 أوتوماتيك.
   شغّل السكريبت مرة واحدة على قاعدة Semou (آمن لو اتشغل تاني).
   ===================================================================== */
USE [Semou]
GO

IF COL_LENGTH('dbo.Bill', 'TaxNo') IS NULL
    ALTER TABLE dbo.Bill ADD TaxNo int NULL;
GO

IF OBJECT_ID('dbo.TaxInvoiceNoSeq', 'SO') IS NULL
    CREATE SEQUENCE dbo.TaxInvoiceNoSeq AS int START WITH 1 INCREMENT BY 1 NO CACHE;
GO

IF OBJECT_ID('dbo.trg_Bill_TaxNo', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Bill_TaxNo;
GO
CREATE TRIGGER dbo.trg_Bill_TaxNo ON dbo.Bill
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @id int;
    DECLARE c CURSOR LOCAL FAST_FORWARD FOR
        SELECT b.ID FROM dbo.Bill b JOIN inserted i ON i.Guid = b.Guid
        WHERE b.TaxNo IS NULL ORDER BY b.ID;
    OPEN c;
    FETCH NEXT FROM c INTO @id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE dbo.Bill SET TaxNo = NEXT VALUE FOR dbo.TaxInvoiceNoSeq WHERE ID = @id;
        FETCH NEXT FROM c INTO @id;
    END
    CLOSE c;
    DEALLOCATE c;
END
GO

/* ---------------------------------------------------------------------
   (اختياري) لو عايز الفواتير اللي اتعملت من 2026/10/01 (تاريخ نفاذ التسجيل)
   لحد النهارده تاخد الترقيم الجديد كمان: شيل التعليق وشغّله مرة واحدة
   فورًا بعد السكريبت وقبل ما تعمل أي فاتورة جديدة.
   ⚠ متشغلوش لو الفواتير دي اتبعتت للهيئة بالفعل بأرقامها القديمة.
   ---------------------------------------------------------------------
DECLARE @id int;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT ID FROM dbo.Bill WHERE TaxNo IS NULL AND BillDate >= '20261001' ORDER BY ID;
OPEN c;
FETCH NEXT FROM c INTO @id;
WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE dbo.Bill SET TaxNo = NEXT VALUE FOR dbo.TaxInvoiceNoSeq WHERE ID = @id;
    FETCH NEXT FROM c INTO @id;
END
CLOSE c;
DEALLOCATE c;
--------------------------------------------------------------------- */

-- مراجعة: آخر الفواتير ورقمها الضريبي
SELECT TOP 20 ID, Code, TaxNo, CASE WHEN TaxNo < 10 THEN '0' ELSE '' END + CAST(TaxNo AS varchar(10)) AS TaxNoPrinted, BillDate
FROM dbo.Bill ORDER BY ID DESC;
GO
