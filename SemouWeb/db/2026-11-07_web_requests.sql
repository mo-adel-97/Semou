/*
 * Web-only review workflow (طلبات الالتحاق / طلبات السداد).
 * Only NEW tables: nothing the desktop reads or writes is changed, so the desktop keeps working as before.
 *
 *   WebFormPermission : permissions of web-only screens. Kept apart from User_Premision because the desktop
 *                       user screen deletes and re-inserts every User_Premision row of a user on each save.
 *   WebRegOrder       : طلب التحاق — the full registration input, reviewed before the RegDoc is created.
 *   WebPayOrder       : طلب سداد  — the full payment-invoice input, reviewed before the Bill is created.
 *   WebOrderFile      : files sent with a request (kept so the reviewer can see them).
 *   WebOrderLog       : timeline of every request (created / edited / approved / rejected ...).
 * Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.WebFormPermission') IS NULL
CREATE TABLE dbo.WebFormPermission (
    ID       int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebFormPermission PRIMARY KEY,
    UserGuid uniqueidentifier NOT NULL,
    FormKey  nvarchar(50)     NOT NULL,
    Add_     bit NOT NULL CONSTRAINT DF_WebFormPermission_Add  DEFAULT 0,
    Edit     bit NOT NULL CONSTRAINT DF_WebFormPermission_Edit DEFAULT 0,
    Del      bit NOT NULL CONSTRAINT DF_WebFormPermission_Del  DEFAULT 0,
    Print_   bit NOT NULL CONSTRAINT DF_WebFormPermission_Prn  DEFAULT 0,
    Find_    bit NOT NULL CONSTRAINT DF_WebFormPermission_Find DEFAULT 0,
    View_    bit NOT NULL CONSTRAINT DF_WebFormPermission_View DEFAULT 0,
    CONSTRAINT UQ_WebFormPermission UNIQUE (UserGuid, FormKey)
);

IF OBJECT_ID('dbo.WebRegOrder') IS NULL
CREATE TABLE dbo.WebRegOrder (
    ID              int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebRegOrder PRIMARY KEY,
    Guid            uniqueidentifier NOT NULL CONSTRAINT DF_WebRegOrder_Guid DEFAULT NEWID() CONSTRAINT UQ_WebRegOrder_Guid UNIQUE,
    Code            int              NOT NULL CONSTRAINT UQ_WebRegOrder_Code UNIQUE,
    OrderDate       datetime         NOT NULL,
    UserGuid        uniqueidentifier NOT NULL,           -- who submitted it
    StudentGuid     uniqueidentifier NOT NULL,
    AccountGuid     uniqueidentifier NOT NULL,
    RegType         int              NOT NULL,           -- 0 دبلوم / 1 دورة
    BranchGuid      uniqueidentifier NOT NULL,           -- study branch
    SellerGuid      uniqueidentifier NULL,
    BatchGuid       uniqueidentifier NULL,
    CourseGuid      uniqueidentifier NULL,
    DiplomGuid      uniqueidentifier NULL,               -- main program (seat reservation)
    Programs        nvarchar(500)    NULL,
    DocTotal        decimal(18,2)    NOT NULL CONSTRAINT DF_WebRegOrder_Total DEFAULT 0,
    DocTax          decimal(18,2)    NOT NULL CONSTRAINT DF_WebRegOrder_Tax DEFAULT 0,
    DocSubTotal     decimal(18,2)    NOT NULL CONSTRAINT DF_WebRegOrder_Sub DEFAULT 0,
    PayAmount       decimal(18,2)    NOT NULL CONSTRAINT DF_WebRegOrder_Pay DEFAULT 0,
    CashAccountGuid uniqueidentifier NULL,
    PayRef          nvarchar(100)    NULL,
    PayDate         datetime         NULL,
    Notes           nvarchar(500)    NULL,
    Payload         nvarchar(max)    NOT NULL,           -- RegistrationInput (JSON)
    Status          int              NOT NULL CONSTRAINT DF_WebRegOrder_Status DEFAULT 0,  -- 0 بانتظار المراجعة / 1 مقبول / 2 مرفوض
    ReviewUserGuid  uniqueidentifier NULL,
    ReviewDate      datetime         NULL,
    ReviewNotes     nvarchar(500)    NULL,
    RegDocGuid      uniqueidentifier NULL,
    RegCode         nvarchar(50)     NULL,
    BillGuid        uniqueidentifier NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebRegOrder_Status')
    CREATE INDEX IX_WebRegOrder_Status ON dbo.WebRegOrder (Status, OrderDate DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebRegOrder_Account')
    CREATE INDEX IX_WebRegOrder_Account ON dbo.WebRegOrder (AccountGuid);

IF OBJECT_ID('dbo.WebPayOrder') IS NULL
CREATE TABLE dbo.WebPayOrder (
    ID              int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebPayOrder PRIMARY KEY,
    Guid            uniqueidentifier NOT NULL CONSTRAINT DF_WebPayOrder_Guid DEFAULT NEWID() CONSTRAINT UQ_WebPayOrder_Guid UNIQUE,
    Code            int              NOT NULL CONSTRAINT UQ_WebPayOrder_Code UNIQUE,
    OrderDate       datetime         NOT NULL,
    UserGuid        uniqueidentifier NOT NULL,
    StudentGuid     uniqueidentifier NOT NULL,
    AccountGuid     uniqueidentifier NOT NULL,
    FileId          int              NOT NULL,           -- StudentLevel.ID
    Kind            int              NOT NULL,           -- 1 سداد / 2 سداد رسوم
    BranchGuid      uniqueidentifier NULL,
    Programs        nvarchar(500)    NULL,
    Amount          decimal(18,2)    NOT NULL CONSTRAINT DF_WebPayOrder_Amount DEFAULT 0,
    CashAccountGuid uniqueidentifier NULL,
    CashAccountGuid2 uniqueidentifier NULL,
    PayRef          nvarchar(100)    NULL,
    PayRef2         nvarchar(100)    NULL,
    PayDate         datetime         NULL,
    Notes           nvarchar(500)    NULL,
    Payload         nvarchar(max)    NOT NULL,           -- PayInvoiceInput (JSON)
    Status          int              NOT NULL CONSTRAINT DF_WebPayOrder_Status DEFAULT 0,  -- 0 بانتظار المراجعة / 1 مؤكد / 2 مكرر / 3 ملغي
    ReviewUserGuid  uniqueidentifier NULL,
    ReviewDate      datetime         NULL,
    ReviewNotes     nvarchar(500)    NULL,
    BillGuid        uniqueidentifier NULL,
    BillCode        nvarchar(50)     NULL,
    TaxNo           nvarchar(20)     NULL,
    FilesArchived   bit              NOT NULL CONSTRAINT DF_WebPayOrder_Archived DEFAULT 0
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebPayOrder_Status')
    CREATE INDEX IX_WebPayOrder_Status ON dbo.WebPayOrder (Status, OrderDate DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebPayOrder_Ref')
    CREATE INDEX IX_WebPayOrder_Ref ON dbo.WebPayOrder (PayRef);

IF OBJECT_ID('dbo.WebOrderFile') IS NULL
CREATE TABLE dbo.WebOrderFile (
    ID          int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebOrderFile PRIMARY KEY,
    Guid        uniqueidentifier NOT NULL CONSTRAINT DF_WebOrderFile_Guid DEFAULT NEWID() CONSTRAINT UQ_WebOrderFile_Guid UNIQUE,
    OrderGuid   uniqueidentifier NULL,                   -- NULL while staged (uploaded before the request is sent)
    StageKey    nvarchar(100)    NULL,                   -- {userGuid}:{nationalId} while staged
    Code        nvarchar(30)     NOT NULL,               -- ID / Certifcat / trans / trans2 / contract / Letter
    FileName    nvarchar(200)    NOT NULL,
    ContentType nvarchar(100)    NOT NULL,
    Size        int              NOT NULL,
    Data        varbinary(max)   NOT NULL,
    UserGuid    uniqueidentifier NOT NULL,
    CreatedAt   datetime         NOT NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebOrderFile_Order')
    CREATE INDEX IX_WebOrderFile_Order ON dbo.WebOrderFile (OrderGuid) INCLUDE (Code, FileName, ContentType, Size);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebOrderFile_Stage')
    CREATE INDEX IX_WebOrderFile_Stage ON dbo.WebOrderFile (StageKey) WHERE StageKey IS NOT NULL;

IF OBJECT_ID('dbo.WebOrderLog') IS NULL
CREATE TABLE dbo.WebOrderLog (
    ID        int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebOrderLog PRIMARY KEY,
    OrderGuid uniqueidentifier NOT NULL,
    Action    nvarchar(50)     NOT NULL,
    UserGuid  uniqueidentifier NOT NULL,
    At        datetime         NOT NULL,
    Notes     nvarchar(1000)   NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebOrderLog_Order')
    CREATE INDEX IX_WebOrderLog_Order ON dbo.WebOrderLog (OrderGuid, At);

/* A registration's down payment becomes a طلب سداد (Kind 0) when the طلب التحاق is approved. */
IF COL_LENGTH('dbo.WebPayOrder', 'RegOrderGuid') IS NULL ALTER TABLE dbo.WebPayOrder ADD RegOrderGuid uniqueidentifier NULL;
IF COL_LENGTH('dbo.WebPayOrder', 'RegDocGuid') IS NULL ALTER TABLE dbo.WebPayOrder ADD RegDocGuid uniqueidentifier NULL;

COMMIT;
