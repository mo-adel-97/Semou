-- Trainee portal (data pushed from the Semou ERP) + study-request sync fields.

CREATE TABLE IF NOT EXISTS portal_students (
  id              INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  erp_guid        CHAR(36) NOT NULL UNIQUE,
  national_id     VARCHAR(20) NOT NULL,
  phone_key       VARCHAR(12) NOT NULL,          -- last 9 digits of the mobile, used as the portal password
  name            VARCHAR(200) NOT NULL,
  code            VARCHAR(40) NULL,
  is_active       TINYINT(1) NOT NULL DEFAULT 1,
  data            JSON NOT NULL,                 -- programs, finance, payments (see lib/erp.php)
  failed_attempts TINYINT UNSIGNED NOT NULL DEFAULT 0,
  locked_until    DATETIME NULL,
  last_login_at   DATETIME NULL,
  synced_at       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY idx_portal_nid (national_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE messages
  ADD COLUMN study_mode   ENUM('onsite','online') NULL AFTER program,
  ADD COLUMN national_id  VARCHAR(20) NULL AFTER email,
  ADD COLUMN city         VARCHAR(80) NULL AFTER national_id,
  ADD COLUMN erp_synced_at DATETIME NULL,
  ADD COLUMN erp_ref      VARCHAR(60) NULL,
  ADD KEY idx_messages_erp (kind, id);

CREATE TABLE IF NOT EXISTS erp_sync_log (
  id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  direction  VARCHAR(20) NOT NULL,             -- students | requests
  items      INT UNSIGNED NOT NULL DEFAULT 0,
  note       VARCHAR(255) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY idx_erp_sync_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
