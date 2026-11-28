/*
 * باقات التسجيل وضبط الأسعار والدفعات المقدمة.
 *   WebPricePlan  الباقات (كاش / 10 شهور / 24 شهر ...): تُضاف وتُعدّل من شاشة «ضبط الأسعار والدفعات المقدمة».
 *   WebPlanPrice  سعر كل دبلوم في كل باقة (السعر النهائي شامل الضريبة) + الدفعة المقدمة (الحد الأدنى عند التسجيل).
 *   WebRegPlan    الباقة التي سُجّل عليها الطالب (نسخة ثابتة من السعر والدفعة والقسط وقت التسجيل) لكشف الحساب وجدول الأقساط.
 * Only NEW objects. Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.WebPricePlan') IS NULL
CREATE TABLE dbo.WebPricePlan (
    Guid        uniqueidentifier NOT NULL CONSTRAINT PK_WebPricePlan PRIMARY KEY CONSTRAINT DF_WebPricePlan_Guid DEFAULT NEWID(),
    Code        int              NOT NULL CONSTRAINT UQ_WebPricePlan_Code UNIQUE,
    Name        nvarchar(100)    NOT NULL CONSTRAINT UQ_WebPricePlan_Name UNIQUE,
    Months      int              NOT NULL CONSTRAINT CK_WebPricePlan_Months CHECK (Months BETWEEN 0 AND 120),  -- 0 = كاش (دفعة واحدة)
    Description nvarchar(250)    NULL,
    Sort        int              NOT NULL CONSTRAINT DF_WebPricePlan_Sort DEFAULT 0,
    IsUse       bit              NOT NULL CONSTRAINT DF_WebPricePlan_IsUse DEFAULT 1,
    CreatedAt   datetime         NOT NULL CONSTRAINT DF_WebPricePlan_Created DEFAULT GETDATE(),
    UpdatedAt   datetime         NOT NULL CONSTRAINT DF_WebPricePlan_Updated DEFAULT GETDATE()
);

IF OBJECT_ID('dbo.WebPlanPrice') IS NULL
CREATE TABLE dbo.WebPlanPrice (
    PlanGuid    uniqueidentifier NOT NULL CONSTRAINT FK_WebPlanPrice_Plan REFERENCES dbo.WebPricePlan (Guid),
    DiplomGuid  uniqueidentifier NOT NULL,
    Price       decimal(18,2)    NOT NULL CONSTRAINT CK_WebPlanPrice_Price CHECK (Price >= 0),
    DownPayment decimal(18,2)    NOT NULL CONSTRAINT DF_WebPlanPrice_Down DEFAULT 0,
    IsUse       bit              NOT NULL CONSTRAINT DF_WebPlanPrice_IsUse DEFAULT 1,  -- الباقة متاحة لهذا الدبلوم
    UpdatedAt   datetime         NOT NULL CONSTRAINT DF_WebPlanPrice_Updated DEFAULT GETDATE(),
    UpdatedBy   uniqueidentifier NULL,
    CONSTRAINT PK_WebPlanPrice PRIMARY KEY (PlanGuid, DiplomGuid),
    CONSTRAINT CK_WebPlanPrice_Down CHECK (DownPayment >= 0 AND DownPayment <= Price)
);

IF OBJECT_ID('dbo.WebRegPlan') IS NULL
CREATE TABLE dbo.WebRegPlan (
    RegDocGuid  uniqueidentifier NOT NULL CONSTRAINT PK_WebRegPlan PRIMARY KEY,   -- RegDoc.Guid
    PlanGuid    uniqueidentifier NOT NULL,
    PlanName    nvarchar(100)    NOT NULL,
    Months      int              NOT NULL,
    Price       decimal(18,2)    NOT NULL,
    DownPayment decimal(18,2)    NOT NULL,
    Installment decimal(18,2)    NOT NULL,   -- (السعر - الدفعة المقدمة) / عدد الشهور
    FirstDue    date             NULL,       -- استحقاق أول قسط (بعد شهر من التسجيل)
    CreatedAt   datetime         NOT NULL CONSTRAINT DF_WebRegPlan_Created DEFAULT GETDATE()
);

-- The three packages the institute starts with (names, months and order are edited from the screen).
IF NOT EXISTS (SELECT 1 FROM dbo.WebPricePlan)
INSERT dbo.WebPricePlan (Code, Name, Months, Description, Sort) VALUES
    (1, N'كاش', 0, N'سداد كامل المبلغ عند التسجيل', 1),
    (2, N'10 شهور', 10, N'دفعة مقدمة ثم 10 أقساط شهرية', 2),
    (3, N'24 شهر', 24, N'دفعة مقدمة ثم 24 قسطاً شهرياً', 3);

COMMIT;
