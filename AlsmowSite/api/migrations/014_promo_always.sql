-- نافذة الإعلان: show it every time the site opens (asked 2026-11-28); still changeable from Settings › نافذة الإعلان.
UPDATE settings SET v = JSON_SET(v, '$.mode', 'always', '$.delay', '1') WHERE k = 'popup';
