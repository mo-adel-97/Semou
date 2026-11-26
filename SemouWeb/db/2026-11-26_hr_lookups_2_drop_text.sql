/*
 * Step 2 of 2026-11-26_hr_lookups.sql: drop the old free-text columns of dbo.UserProfile
 * (their values were moved into HrJobTitle / HrEmploymentType / HrSpecialization / HrBank).
 * Run only after the web app that uses the *Guid columns is deployed. Safe to run more than once.
 */
SET XACT_ABORT ON;
IF COL_LENGTH('dbo.UserProfile', 'JobTitle') IS NOT NULL
    ALTER TABLE dbo.UserProfile DROP COLUMN JobTitle, EmploymentType, Specialization, BankName;
