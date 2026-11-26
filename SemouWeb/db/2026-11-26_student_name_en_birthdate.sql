/*
 * بطاقة الطالب: الاسم بالإنجليزي + تاريخ الميلاد.
 * Two NULLable columns on dbo.Student; the AddStudent / UpdateStudent procs (shared with the desktop) are untouched:
 * the web backend fills these columns with a separate UPDATE inside the same transaction.
 * Safe to run more than once.
 */
SET XACT_ABORT ON;
BEGIN TRAN;
IF COL_LENGTH('dbo.Student', 'StudentNameEn') IS NULL ALTER TABLE dbo.Student ADD StudentNameEn nvarchar(150) NULL;
IF COL_LENGTH('dbo.Student', 'BirthDate') IS NULL ALTER TABLE dbo.Student ADD BirthDate date NULL;
COMMIT;
