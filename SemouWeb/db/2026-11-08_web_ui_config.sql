/*
 * Web-only "الإعدادات العامة" (General settings): sidebar structure and interface texts.
 * Only a NEW table: nothing the desktop reads or writes is changed.
 *
 *   WebUiConfig : every saved version of a settings section (menu / texts) as JSON. The current one is the
 *                 latest row of the section; older rows are the history the screen can restore from.
 * Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.WebUiConfig') IS NULL
CREATE TABLE dbo.WebUiConfig (
    ID       int IDENTITY(1,1) NOT NULL CONSTRAINT PK_WebUiConfig PRIMARY KEY,
    Section  nvarchar(30)     NOT NULL,
    Json     nvarchar(max)    NOT NULL,
    UserGuid uniqueidentifier NOT NULL,
    SavedAt  datetime         NOT NULL,
    Note     nvarchar(200)    NULL
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WebUiConfig_Section' AND object_id = OBJECT_ID('dbo.WebUiConfig'))
CREATE INDEX IX_WebUiConfig_Section ON dbo.WebUiConfig (Section, ID DESC);

COMMIT;
