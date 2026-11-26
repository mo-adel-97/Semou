-- Search, round 2 (Search Console queries + the phrases people type in Saudi Arabia). Adds only what is missing,
-- so anything edited from the dashboard (الإعدادات ← محركات البحث / التواصل) stays as it is.

-- postal code of Hafr Al-Batin: the institute's PostalAddress in schema.org (Google warned it was missing)
UPDATE settings SET v = JSON_SET(v, '$.postal_code', '39921')
WHERE k = 'contact' AND (JSON_EXTRACT(v, '$.postal_code') IS NULL OR JSON_UNQUOTE(JSON_EXTRACT(v, '$.postal_code')) = '');

-- the home page title in Google names the city people search with (only if it is still the seeded one)
UPDATE settings SET v = JSON_SET(v, '$.title', 'معهد السمو للتدريب في حفر الباطن | دبلومات ودورات معتمدة')
WHERE k = 'seo' AND JSON_UNQUOTE(JSON_EXTRACT(v, '$.title')) = 'معهد السمو للتدريب | دبلومات ودورات معتمدة';

-- names people search the institute by (Google Business Profile name, city, short forms)
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.alt_names', 'معهد السمو العالي المتخصص للتدريب واللغات')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.alt_names')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.alt_names')) < 10
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.alt_names'), JSON_QUOTE('معهد السمو العالي المتخصص للتدريب واللغات'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.alt_names', 'معهد السمو حفر الباطن')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.alt_names')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.alt_names')) < 10
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.alt_names'), JSON_QUOTE('معهد السمو حفر الباطن'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.alt_names', 'معهد سمو')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.alt_names')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.alt_names')) < 10
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.alt_names'), JSON_QUOTE('معهد سمو'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.alt_names', 'معهد السمو للتدريب والتعليم')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.alt_names')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.alt_names')) < 10
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.alt_names'), JSON_QUOTE('معهد السمو للتدريب والتعليم'));

-- phrases people search with
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'معاهد حفر الباطن')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('معاهد حفر الباطن'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'معاهد حفر الباطن للنساء')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('معاهد حفر الباطن للنساء'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'معهد نسائي في حفر الباطن')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('معهد نسائي في حفر الباطن'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم بعد الثانوي')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم بعد الثانوي'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم سنتين معتمد')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم سنتين معتمد'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم عن بعد معتمد من التقني')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم عن بعد معتمد من التقني'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم موارد بشرية عن بعد')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم موارد بشرية عن بعد'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم إدارة مكتبية عن بعد')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم إدارة مكتبية عن بعد'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دبلوم إدارة مستشفيات عن بعد')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دبلوم إدارة مستشفيات عن بعد'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دورات معتمدة في حفر الباطن')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دورات معتمدة في حفر الباطن'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'معهد معتمد من التدريب التقني والمهني')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('معهد معتمد من التدريب التقني والمهني'));
UPDATE settings SET v = JSON_ARRAY_APPEND(v, '$.keywords', 'دورات مدعومة من هدف وتمهير')
WHERE k = 'seo' AND JSON_TYPE(JSON_EXTRACT(v, '$.keywords')) = 'ARRAY' AND JSON_LENGTH(JSON_EXTRACT(v, '$.keywords')) < 30
  AND NOT JSON_CONTAINS(JSON_EXTRACT(v, '$.keywords'), JSON_QUOTE('دورات مدعومة من هدف وتمهير'));
