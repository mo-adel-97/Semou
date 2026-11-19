# Semou Web

The web version of the Semou desktop system (VB.NET WinForms). It uses **the same SQL Server database** with the same stored procedures, permissions, and audit log, so the desktop app and the web app can run side by side.

| Folder | Tech |
|---|---|
| `api/` | .NET 8 Web API, Dapper, JWT, ProblemDetails, rate limiting, health check |
| `web/` | React 19, TypeScript, Vite 8. RTL, Cairo font, alsmow.com brand colors |

### Frontend libraries
| Library | Role |
|---|---|
| TanStack Query v5 | Server data: caching, background refresh, retry, automatic errors |
| TanStack Table v9 | Grids: search, sorting, pagination (`components/DataTable.tsx`) |
| React Hook Form + Zod v4 | Forms and validation with Arabic messages |
| SweetAlert2 | Every message: success, error, confirmation, edit reason, toasts (`lib/alerts.ts`) |
| Motion | Animations: page transitions, drawer, menus |
| cmdk | Quick search Ctrl+K and pick-list dialogs |
| React Router v7, lucide-react | Routing and icons |

### Themes
Light / dark / automatic (follows the device) + 8 colors (السمو, محيطي, ملكي, تركواز, وردي, غروب, ذهبي, فحمي) from the palette button in the top bar.
Everything uses CSS variables in `index.css`: each color defines only `--brand` and every shade is derived automatically in both modes.
To add a new color: one line in `index.css` (`[data-accent="x"] { --brand: ...; --brand-glow: ...; }`) + one entry in `ACCENTS` in `theme.tsx`.

## Running locally

```bash
# 1) Secrets: once per machine, never stored in git
cd api
dotnet user-secrets set "ConnectionStrings:Semou" "Server=...;Database=Semou;User Id=...;Password=...;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:Key" "<random value, 32+ characters>"

# 2) API
dotnet run --urls http://localhost:5080

# 3) Frontend (in a second terminal)
cd ../web
npm install
npm run dev        # http://localhost:5173
```

## Publishing to a server

```bash
cd web && npm run build          # outputs to api/wwwroot
cd ../api && dotnet publish -c Release -o ../publish
```
On the server, set `ConnectionStrings__Semou` and `Jwt__Key` as environment variables, and put the site behind HTTPS.

## Backend structure

```
api/
  Data/Db.cs                    connections + InTransactionAsync
  Security/PermissionService    User_Premision + UserMenu (same rules as AllFiles.FORxxx / MENU_VIEW__)
  Security/RequirePermission    [RequirePermission("addbranch", Perm.Edit)] enforced on the server
  Infrastructure/               unified error handling, audit log (Add_UserAction), Saudi time
  Features/<Module>/            Controller -> Service -> Repository for each desktop module
```

## How permissions work (same as the desktop app)
- **Main menus**: `MainMenu` + `UserMenu`. A menu is hidden only when `View_ = 0`.
- **Screens**: `Form_Name.UnqName` + `User_Premision` (Add_/Edit/Del/Print_/Find_/View_). No row means no permission.
- The UI hides what the user can't use, and the API rejects any request without the permission (403) even if called directly.
- Every add or edit is recorded in `User_Action`, and every edit requires a reason (same as `UpdateResaon`).

## Porting a new screen (fixed recipe)
1. Read the original form in `SEMOU/SSTLI/Form/...` and the stored procedures it uses.
2. `api/Features/<Module>/`: Models (validation in Arabic) + Repository (same procs) + Service (rules + transaction + audit) + Controller (`[RequirePermission]`).
3. `web/src/pages/<Screen>.tsx` using `DataTable` + `Drawer` + `Field`.
4. Add it to `SCREENS` in `App.tsx` and set `ported: true` in `menu.ts`.

## Status
- [x] Login (same User_Info rules + stopped users), session, logout
- [x] Full menu (6 sections, ~75 screens) filtered by each user's permissions
- [x] Branches (search, add, edit with reason, cost-center picker)
- [x] Change password
- [ ] Remaining screens (see `menu.ts`: anything without `ported: true`)
- [ ] Printing / reports (DevExpress Reports): needs a replacement (PDF from the server)
