/*
 * بيانات الموظف: قوائم مُدارة بدل النص الحر (للإحصائيات):
 *   HrJobTitle        المسميات الوظيفية
 *   HrEmploymentType  الحالات الوظيفية (أساسي / متعاون / جزئي ...)
 *   HrSpecialization  التخصصات والمؤهلات
 *   HrBank            البنوك (+ رمز البنك داخل الآيبان السعودي، يُختار البنك تلقائياً من الآيبان)
 * Same shape as the desktop lookup tables (ID / Code / Guid / IsUse + Add*/Update* procs), so the web screens engine runs them.
 * dbo.UserProfile: the free-text columns become references (existing text is moved into the lists first).
 * Only NEW objects; nothing the desktop uses is touched. Safe to run more than once.
 */
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* ---------------- tables ---------------- */
IF OBJECT_ID('dbo.HrJobTitle') IS NULL
CREATE TABLE dbo.HrJobTitle (
    ID int NOT NULL CONSTRAINT PK_HrJobTitle PRIMARY KEY, Code nvarchar(50) NOT NULL,
    Guid uniqueidentifier NOT NULL CONSTRAINT DF_HrJobTitle_Guid DEFAULT NEWID() CONSTRAINT UQ_HrJobTitle_Guid UNIQUE,
    Name nvarchar(150) NOT NULL CONSTRAINT UQ_HrJobTitle_Name UNIQUE, Notes nvarchar(250) NULL, IsUse bit NOT NULL CONSTRAINT DF_HrJobTitle_IsUse DEFAULT 1);
IF OBJECT_ID('dbo.HrEmploymentType') IS NULL
CREATE TABLE dbo.HrEmploymentType (
    ID int NOT NULL CONSTRAINT PK_HrEmploymentType PRIMARY KEY, Code nvarchar(50) NOT NULL,
    Guid uniqueidentifier NOT NULL CONSTRAINT DF_HrEmploymentType_Guid DEFAULT NEWID() CONSTRAINT UQ_HrEmploymentType_Guid UNIQUE,
    Name nvarchar(150) NOT NULL CONSTRAINT UQ_HrEmploymentType_Name UNIQUE, Notes nvarchar(250) NULL, IsUse bit NOT NULL CONSTRAINT DF_HrEmploymentType_IsUse DEFAULT 1);
IF OBJECT_ID('dbo.HrSpecialization') IS NULL
CREATE TABLE dbo.HrSpecialization (
    ID int NOT NULL CONSTRAINT PK_HrSpecialization PRIMARY KEY, Code nvarchar(50) NOT NULL,
    Guid uniqueidentifier NOT NULL CONSTRAINT DF_HrSpecialization_Guid DEFAULT NEWID() CONSTRAINT UQ_HrSpecialization_Guid UNIQUE,
    Name nvarchar(150) NOT NULL CONSTRAINT UQ_HrSpecialization_Name UNIQUE, Notes nvarchar(250) NULL, IsUse bit NOT NULL CONSTRAINT DF_HrSpecialization_IsUse DEFAULT 1);
IF OBJECT_ID('dbo.HrBank') IS NULL
CREATE TABLE dbo.HrBank (
    ID int NOT NULL CONSTRAINT PK_HrBank PRIMARY KEY, Code nvarchar(50) NOT NULL,
    Guid uniqueidentifier NOT NULL CONSTRAINT DF_HrBank_Guid DEFAULT NEWID() CONSTRAINT UQ_HrBank_Guid UNIQUE,
    Name nvarchar(150) NOT NULL CONSTRAINT UQ_HrBank_Name UNIQUE,
    IbanCode nvarchar(2) NULL,           -- characters 5-6 of a Saudi IBAN
    Notes nvarchar(250) NULL, IsUse bit NOT NULL CONSTRAINT DF_HrBank_IsUse DEFAULT 1);
GO

/* ---------------- procs (the shape the screens engine expects) ---------------- */
CREATE OR ALTER PROC dbo.AddHrJobTitle @ID int, @Code nvarchar(50), @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    INSERT dbo.HrJobTitle (ID, Code, Name, Notes, IsUse) VALUES (@ID, @Code, LTRIM(RTRIM(@Name)), @Notes, @IsUse);
GO
CREATE OR ALTER PROC dbo.UpdateHrJobTitle @Guid uniqueidentifier, @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    UPDATE dbo.HrJobTitle SET Name = LTRIM(RTRIM(@Name)), Notes = @Notes, IsUse = @IsUse WHERE Guid = @Guid;
GO
CREATE OR ALTER PROC dbo.AddHrEmploymentType @ID int, @Code nvarchar(50), @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    INSERT dbo.HrEmploymentType (ID, Code, Name, Notes, IsUse) VALUES (@ID, @Code, LTRIM(RTRIM(@Name)), @Notes, @IsUse);
GO
CREATE OR ALTER PROC dbo.UpdateHrEmploymentType @Guid uniqueidentifier, @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    UPDATE dbo.HrEmploymentType SET Name = LTRIM(RTRIM(@Name)), Notes = @Notes, IsUse = @IsUse WHERE Guid = @Guid;
GO
CREATE OR ALTER PROC dbo.AddHrSpecialization @ID int, @Code nvarchar(50), @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    INSERT dbo.HrSpecialization (ID, Code, Name, Notes, IsUse) VALUES (@ID, @Code, LTRIM(RTRIM(@Name)), @Notes, @IsUse);
GO
CREATE OR ALTER PROC dbo.UpdateHrSpecialization @Guid uniqueidentifier, @Name nvarchar(150), @Notes nvarchar(250), @IsUse bit AS
    UPDATE dbo.HrSpecialization SET Name = LTRIM(RTRIM(@Name)), Notes = @Notes, IsUse = @IsUse WHERE Guid = @Guid;
GO
CREATE OR ALTER PROC dbo.AddHrBank @ID int, @Code nvarchar(50), @Name nvarchar(150), @IbanCode nvarchar(2), @Notes nvarchar(250), @IsUse bit AS
    INSERT dbo.HrBank (ID, Code, Name, IbanCode, Notes, IsUse) VALUES (@ID, @Code, LTRIM(RTRIM(@Name)), NULLIF(LTRIM(RTRIM(@IbanCode)), ''), @Notes, @IsUse);
GO
CREATE OR ALTER PROC dbo.UpdateHrBank @Guid uniqueidentifier, @Name nvarchar(150), @IbanCode nvarchar(2), @Notes nvarchar(250), @IsUse bit AS
    UPDATE dbo.HrBank SET Name = LTRIM(RTRIM(@Name)), IbanCode = NULLIF(LTRIM(RTRIM(@IbanCode)), ''), Notes = @Notes, IsUse = @IsUse WHERE Guid = @Guid;
GO

/* ---------------- UserProfile: text -> references ---------------- */
BEGIN TRAN;
IF COL_LENGTH('dbo.UserProfile', 'JobTitleGuid') IS NULL ALTER TABLE dbo.UserProfile ADD JobTitleGuid uniqueidentifier NULL;
IF COL_LENGTH('dbo.UserProfile', 'EmploymentTypeGuid') IS NULL ALTER TABLE dbo.UserProfile ADD EmploymentTypeGuid uniqueidentifier NULL;
IF COL_LENGTH('dbo.UserProfile', 'SpecializationGuid') IS NULL ALTER TABLE dbo.UserProfile ADD SpecializationGuid uniqueidentifier NULL;
IF COL_LENGTH('dbo.UserProfile', 'BankGuid') IS NULL ALTER TABLE dbo.UserProfile ADD BankGuid uniqueidentifier NULL;
COMMIT;
GO

-- Saudi banks and their IBAN codes (editable from شاشة البنوك).
IF NOT EXISTS (SELECT 1 FROM dbo.HrBank)
INSERT dbo.HrBank (ID, Code, Name, IbanCode) VALUES
    (1, '1', N'مصرف الراجحي', '80'), (2, '2', N'البنك الأهلي السعودي', '10'), (3, '3', N'بنك الرياض', '20'),
    (4, '4', N'مصرف الإنماء', '05'), (5, '5', N'بنك البلاد', '15'), (6, '6', N'البنك العربي الوطني', '30'),
    (7, '7', N'البنك السعودي الأول (ساب)', '45'), (8, '8', N'البنك السعودي الفرنسي', '55'), (9, '9', N'بنك الجزيرة', '60'),
    (10, '10', N'البنك السعودي للاستثمار', '65');
GO

-- Move the free text that was typed before into the lists (the text columns are dropped by 2026-11-26_hr_lookups_2_drop_text.sql,
-- run after the web app that reads the references is deployed).
IF COL_LENGTH('dbo.UserProfile', 'JobTitle') IS NOT NULL
BEGIN
    BEGIN TRAN;
    EXEC(N'
    INSERT dbo.HrJobTitle (ID, Code, Name)
    SELECT ISNULL((SELECT MAX(ID) FROM dbo.HrJobTitle), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V), CAST(ISNULL((SELECT MAX(ID) FROM dbo.HrJobTitle), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V) AS nvarchar(50)), V
    FROM (SELECT LTRIM(RTRIM(JobTitle)) V FROM dbo.UserProfile WHERE NULLIF(LTRIM(JobTitle), '''') IS NOT NULL) X
    WHERE NOT EXISTS (SELECT 1 FROM dbo.HrJobTitle H WHERE H.Name = X.V) GROUP BY V;
    INSERT dbo.HrEmploymentType (ID, Code, Name)
    SELECT ISNULL((SELECT MAX(ID) FROM dbo.HrEmploymentType), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V), CAST(ISNULL((SELECT MAX(ID) FROM dbo.HrEmploymentType), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V) AS nvarchar(50)), V
    FROM (SELECT LTRIM(RTRIM(EmploymentType)) V FROM dbo.UserProfile WHERE NULLIF(LTRIM(EmploymentType), '''') IS NOT NULL) X
    WHERE NOT EXISTS (SELECT 1 FROM dbo.HrEmploymentType H WHERE H.Name = X.V) GROUP BY V;
    INSERT dbo.HrSpecialization (ID, Code, Name)
    SELECT ISNULL((SELECT MAX(ID) FROM dbo.HrSpecialization), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V), CAST(ISNULL((SELECT MAX(ID) FROM dbo.HrSpecialization), 0) + ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC, V) AS nvarchar(50)), V
    FROM (SELECT LTRIM(RTRIM(Specialization)) V FROM dbo.UserProfile WHERE NULLIF(LTRIM(Specialization), '''') IS NOT NULL) X
    WHERE NOT EXISTS (SELECT 1 FROM dbo.HrSpecialization H WHERE H.Name = X.V) GROUP BY V;
    INSERT dbo.HrBank (ID, Code, Name)
    SELECT ISNULL((SELECT MAX(ID) FROM dbo.HrBank), 0) + ROW_NUMBER() OVER (ORDER BY V), CAST(ISNULL((SELECT MAX(ID) FROM dbo.HrBank), 0) + ROW_NUMBER() OVER (ORDER BY V) AS nvarchar(50)), V
    FROM (SELECT LTRIM(RTRIM(BankName)) V FROM dbo.UserProfile WHERE NULLIF(LTRIM(BankName), '''') IS NOT NULL) X
    WHERE NOT EXISTS (SELECT 1 FROM dbo.HrBank H WHERE H.Name = X.V) GROUP BY V;

    UPDATE P SET JobTitleGuid = (SELECT Guid FROM dbo.HrJobTitle H WHERE H.Name = LTRIM(RTRIM(P.JobTitle))),
                 EmploymentTypeGuid = (SELECT Guid FROM dbo.HrEmploymentType H WHERE H.Name = LTRIM(RTRIM(P.EmploymentType))),
                 SpecializationGuid = (SELECT Guid FROM dbo.HrSpecialization H WHERE H.Name = LTRIM(RTRIM(P.Specialization))),
                 BankGuid = (SELECT Guid FROM dbo.HrBank H WHERE H.Name = LTRIM(RTRIM(P.BankName)))
    FROM dbo.UserProfile P;');
    COMMIT;
END
GO
