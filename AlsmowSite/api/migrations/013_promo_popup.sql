-- نافذة الإعلان: the healthcare-diplomas campaign (pictures in web/public/promo), on, leading to the registration page.
-- Only while the group was never saved; afterwards everything is edited from the dashboard (Settings › نافذة الإعلان).
INSERT INTO settings (k, v)
SELECT 'popup', JSON_OBJECT('enabled', TRUE, 'image_desktop', '/promo/health-desktop.webp', 'image_mobile', '/promo/health-mobile.webp',
                            'link', '/register', 'cta', 'سجّل الآن', 'alt', 'خطوتك نحو مستقبل قيادي في القطاع الصحي: إدارة المستشفيات والخدمات الصحية وإدارة التمريض بخصم 5,000 ريال', 'mode', 'session')
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM settings WHERE k = 'popup');
