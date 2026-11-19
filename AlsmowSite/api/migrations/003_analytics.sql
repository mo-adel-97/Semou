-- Site analytics (lib/analytics.php): privacy-friendly, no cookies.
-- visitor = daily hash of (ip, user agent, salt): counts unique visitors per day without storing the IP.
CREATE TABLE IF NOT EXISTS analytics_events (
  id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  at         DATETIME NOT NULL,
  day        DATE NOT NULL,
  kind       VARCHAR(12) NOT NULL,          -- view | leave | event
  name       VARCHAR(40) NULL,              -- event name: chat_open, register_click, whatsapp_click…
  path       VARCHAR(300) NOT NULL,
  title      VARCHAR(200) NULL,
  ref_host   VARCHAR(120) NULL,
  source     VARCHAR(20) NULL,              -- direct | search | social | referral | campaign
  utm_source VARCHAR(80) NULL,
  utm_medium VARCHAR(80) NULL,
  utm_campaign VARCHAR(120) NULL,
  visitor    CHAR(16) NOT NULL,
  session    CHAR(16) NOT NULL,
  is_new     TINYINT(1) NOT NULL DEFAULT 0,
  entry      TINYINT(1) NOT NULL DEFAULT 0, -- first view of the session
  device     VARCHAR(10) NULL,              -- mobile | tablet | desktop
  browser    VARCHAR(20) NULL,
  os         VARCHAR(20) NULL,
  lang       VARCHAR(10) NULL,
  country    CHAR(2) NULL,
  screen_w   SMALLINT UNSIGNED NULL,
  duration   INT UNSIGNED NULL,             -- seconds on the page (kind = leave)
  KEY idx_at (at),
  KEY idx_day_kind (day, kind),
  KEY idx_session (session)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
