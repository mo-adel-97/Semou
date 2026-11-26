-- Search: the names people use for the institute and the phrases it should be found by. Filled only while empty;
-- both lists are edited afterwards from the dashboard (الإعدادات ← محركات البحث). They feed the schema.org
-- alternateName / knowsAbout of the institute on every page (web/public/seo.php).
UPDATE settings SET v = JSON_SET(v, '$.alt_names', JSON_ARRAY(
  'معهد السمو', 'السمو للتدريب', 'معهد السمو المتخصص العالي للتدريب', 'معهد السمو العالي للتدريب', 'Al Sumou Institute', 'Alsmow'))
WHERE k = 'seo' AND (JSON_EXTRACT(v, '$.alt_names') IS NULL OR JSON_LENGTH(JSON_EXTRACT(v, '$.alt_names')) = 0);

UPDATE settings SET v = JSON_SET(v, '$.keywords', JSON_ARRAY(
  'معهد السمو', 'معهد تدريب في حفر الباطن', 'دبلومات معتمدة', 'دبلوم معتمد عن بعد',
  'دبلومات معتمدة من المؤسسة العامة للتدريب التقني والمهني', 'دورات تدريبية معتمدة', 'شهادات معتمدة',
  'دبلوم إدارة الموارد البشرية', 'دبلوم الإدارة المكتبية', 'دبلوم إدارة المستشفيات والخدمات الصحية',
  'دبلوم إدارة التمريض', 'دبلوم القانون', 'دورة الذكاء الاصطناعي', 'دورة الحاسب الآلي',
  'دورة إدخال البيانات', 'دورة اللغة الإنجليزية', 'دورات تأهيلية لسوق العمل', 'دورات تطويرية'))
WHERE k = 'seo' AND (JSON_EXTRACT(v, '$.keywords') IS NULL OR JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) = 0);
