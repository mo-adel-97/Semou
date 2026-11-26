-- Programme field (مجال البرنامج): picks the live "real world" scene the site draws for the programme
-- (web/src/site/home/scenes.tsx) and its icon; careers = the job titles shown under it (optional).
ALTER TABLE programs ADD COLUMN category VARCHAR(30) NULL AFTER type, ADD COLUMN careers VARCHAR(200) NULL AFTER category;

-- first guess for the programmes already published (reviewed afterwards from the dashboard)
UPDATE programs SET category = CASE
  WHEN title LIKE '%مستشفي%' OR title LIKE '%صحي%' THEN 'health'
  WHEN title LIKE '%تمريض%' THEN 'nursing'
  WHEN title LIKE '%ذكاء%' OR title LIKE '%اصطناع%' THEN 'ai'
  WHEN title LIKE '%قانون%' THEN 'law'
  WHEN title LIKE '%موارد%' THEN 'hr'
  WHEN title LIKE '%بيانات%' THEN 'data'
  WHEN title LIKE '%حاسب%' THEN 'computer'
  WHEN title LIKE '%مكتب%' THEN 'office'
  WHEN title LIKE '%انجليز%' OR title LIKE '%إنجليز%' OR title LIKE '%لغة%' THEN 'language'
  WHEN title LIKE '%محاسب%' THEN 'accounting'
  WHEN title LIKE '%تسويق%' OR title LIKE '%مبيعات%' THEN 'marketing'
  ELSE 'business' END
WHERE category IS NULL
