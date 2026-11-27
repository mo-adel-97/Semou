-- «السمو» alone is how many people search the institute: it becomes one of its names (schema.org alternateName of the
-- institute and of the website, which Google uses for the site name). Replaces a name nobody searches with.
UPDATE settings SET v = JSON_REPLACE(v, JSON_UNQUOTE(JSON_SEARCH(v, 'one', 'معهد السمو للتدريب والتعليم', NULL, '$.alt_names')), 'السمو')
WHERE k = 'seo' AND JSON_SEARCH(v, 'one', 'معهد السمو للتدريب والتعليم', NULL, '$.alt_names') IS NOT NULL
  AND JSON_SEARCH(v, 'one', 'السمو', NULL, '$.alt_names') IS NULL;
