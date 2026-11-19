# Alsmow site (alsmow.com)

React site + content dashboard for معهد السمو للتدريب. Runs on Hostinger shared hosting: PHP 8.2 + MySQL, no Node on the server.

| Folder | What |
|---|---|
| `web/` | React 19 + TypeScript + Vite + Tailwind 4. Public site + dashboard at `/admin` (lazy-loaded). |
| `api/` | PHP API (`/api/*`), no framework. One content definition in `lib/resources.php` drives DB whitelist, validation and the dashboard forms. |
| `api/tools/install.php` | CLI: `migrate`, `seed`, `admin`, `import-wp` (SSH only). |
| `deploy.sh` | Build + upload + migrate in one command. |

## Server setup (once)
1. `api/config.php` from `api/config.sample.php` (DB credentials + random `jwt_secret`). Never committed.
2. `php api/tools/install.php migrate && php api/tools/install.php seed`
3. `ADMIN_PASSWORD='...' php api/tools/install.php admin you@example.com "الاسم"`
4. Optional: `php api/tools/install.php import-wp ~/domains/alsmow.com/public_html/wp-config.php ~/domains/alsmow.com/public_html/wp-content/uploads`

## Local development
`cd web && npm install && npm run dev` (http://localhost:5180). `/api` is proxied to the staging server.

## Adding a new content type
Add it to `resources()` in `api/lib/resources.php` + a migration. The dashboard picks it up automatically.
