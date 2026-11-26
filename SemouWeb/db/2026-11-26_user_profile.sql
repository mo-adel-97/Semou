/*
 * بطاقة المستخدم: بيانات الموظف (الهوية، الوظيفة، الحالة الوظيفية، التخصص، الجوالات، البريد، الآيبان...).
 * Only a NEW table: dbo.User_Info and the desktop procs stay untouched. One row per user (UserGuid = User_Info.Guid).
 * Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.UserProfile') IS NULL
CREATE TABLE dbo.UserProfile (
    UserGuid        uniqueidentifier NOT NULL CONSTRAINT PK_UserProfile PRIMARY KEY,
    NationalId      nvarchar(10)     NULL,   -- رقم الهوية / الإقامة
    JobTitle        nvarchar(100)    NULL,   -- الوظيفة
    EmploymentType  nvarchar(50)     NULL,   -- الحالة الوظيفية (أساسي / متعاون / جزئي ...)
    Specialization  nvarchar(150)    NULL,   -- التخصص / المؤهل
    PersonalMobile  nvarchar(10)     NULL,
    WorkMobile      nvarchar(10)     NULL,
    Email           nvarchar(150)    NULL,
    Iban            nvarchar(34)     NULL,   -- stored without spaces
    BankName        nvarchar(100)    NULL,
    HireDate        date             NULL,   -- تاريخ المباشرة
    BirthDate       date             NULL,
    Notes           nvarchar(500)    NULL,
    UpdatedAt       datetime         NOT NULL CONSTRAINT DF_UserProfile_Updated DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_UserProfile_NationalId')
CREATE UNIQUE INDEX UX_UserProfile_NationalId ON dbo.UserProfile (NationalId) WHERE NationalId IS NOT NULL;

COMMIT;
