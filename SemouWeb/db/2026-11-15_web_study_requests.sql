/*
 * طلبات الدراسة من موقع السمو (alsmow.com) — حضوري / عن بعد.
 * Only NEW tables: nothing the desktop reads or writes is changed.
 *
 *   WebStudyRequest : every "سجلي الآن" request submitted on the website, pulled by the ERP web backend
 *                     (Website sync, HMAC-signed) every minute. Sales follow up on it from المبيعات ← طلبات الدراسة.
 *   WebSyncState    : sync cursors (last pulled request id, last portal push...).
 * Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.WebStudyRequest') IS NULL
CREATE TABLE dbo.WebStudyRequest (
    ID              int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebStudyRequest PRIMARY KEY,
    Guid            uniqueidentifier NOT NULL CONSTRAINT DF_WebStudyRequest_Guid DEFAULT NEWID() CONSTRAINT UQ_WebStudyRequest_Guid UNIQUE,
    SiteId          int              NOT NULL CONSTRAINT UQ_WebStudyRequest_SiteId UNIQUE,  -- id on the website (idempotent pull)
    RequestDate     datetime         NOT NULL,                                               -- when the visitor submitted it (KSA time)
    ReceivedAt      datetime         NOT NULL CONSTRAINT DF_WebStudyRequest_Received DEFAULT GETDATE(),
    StudentName     nvarchar(120)    NOT NULL,
    Phone           nvarchar(30)     NULL,
    Email           nvarchar(190)    NULL,
    NationalId      nvarchar(20)     NULL,
    City            nvarchar(80)     NULL,
    Program         nvarchar(220)    NULL,
    StudyMode       tinyint          NOT NULL CONSTRAINT DF_WebStudyRequest_Mode DEFAULT 0,  -- 1 حضوري / 2 عن بعد / 0 غير محدد
    Notes           nvarchar(max)    NULL,                                                   -- what the visitor wrote
    Status          int              NOT NULL CONSTRAINT DF_WebStudyRequest_Status DEFAULT 0,-- 0 جديد / 1 جاري التواصل / 2 تم التسجيل / 3 غير مهتم
    FollowNotes     nvarchar(1000)   NULL,                                                   -- sales follow-up notes
    StudentGuid     uniqueidentifier NULL,                                                   -- existing student with the same national ID
    UpdatedAt       datetime         NULL,
    UpdatedUserGuid uniqueidentifier NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebStudyRequest_Mode')
    CREATE INDEX IX_WebStudyRequest_Mode ON dbo.WebStudyRequest (StudyMode, Status, RequestDate DESC);

IF OBJECT_ID('dbo.WebSyncState') IS NULL
CREATE TABLE dbo.WebSyncState (
    Name      nvarchar(50)  NOT NULL CONSTRAINT PK_WebSyncState PRIMARY KEY,
    Value     nvarchar(200) NULL,
    UpdatedAt datetime      NOT NULL CONSTRAINT DF_WebSyncState_Updated DEFAULT GETDATE()
);

COMMIT;
