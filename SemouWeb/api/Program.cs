using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Semou.Api.Data;
using Semou.Api.Features.Auth;
using Semou.Api.Features.Branches;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

var builder = WebApplication.CreateBuilder(args);
// Runs as a Windows service on the ERP machine (no effect under `dotnet run`).
builder.Host.UseWindowsService();
var cfg = builder.Configuration;

// ---------- Configuration (secrets come from user-secrets / environment variables, never from git) ----------
var jwt = cfg.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (jwt.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be set (at least 32 characters). Use dotnet user-secrets or the Jwt__Key environment variable.");
builder.Services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

// Test mode: a separate copy of the database (Semou_Test) for safe trials. Refuse to run against the real DB.
var testMode = cfg.GetValue("App:TestMode", false);
if (testMode)
    cfg["ConnectionStrings:Semou"] = cfg.GetConnectionString("SemouTest") is { Length: > 0 } t ? t
        : throw new InvalidOperationException("App:TestMode needs ConnectionStrings:SemouTest (user-secrets).");
var dbName = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cfg.GetConnectionString("Semou") ?? "").InitialCatalog;
if (testMode && !dbName.EndsWith("_Test", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException($"App:TestMode is on but the database is '{dbName}'. Test mode must use a *_Test database.");

// ---------- Core ----------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddSemouErrorHandling();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// ---------- Data + security ----------
builder.Services.AddSingleton<Db>();
builder.Services.AddSingleton<PermissionService>();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<UserActionLog>();

// ---------- Features (one folder per desktop module) ----------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BranchRepository>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddSingleton<Semou.Api.Features.Screens.ProcCatalog>();
builder.Services.AddScoped<Semou.Api.Features.Users.UsersService>();
builder.Services.AddScoped<Semou.Api.Features.Accounting.ChartService>();
builder.Services.AddScoped<Semou.Api.Features.Accounting.CashBoxService>();
builder.Services.AddScoped<Semou.Api.Features.Vouchers.VoucherService>();
builder.Services.AddScoped<Semou.Api.Features.Vouchers.JournalService>();
builder.Services.AddScoped<Semou.Api.Features.Vouchers.TransferService>();
builder.Services.AddSingleton<ArchiveDb>();
builder.Services.AddScoped<Semou.Api.Features.Reception.ReceptionService>();
builder.Services.AddScoped<Semou.Api.Features.Reception.AcceptanceService>();
builder.Services.AddScoped<Semou.Api.Features.Reception.StatementService>();
builder.Services.AddScoped<Semou.Api.Features.Fees.FeeFormService>();
builder.Services.AddScoped<Semou.Api.Features.Reception.StudentActionsService>();
builder.Services.AddScoped<Semou.Api.Features.Returns.ReturnsService>();
builder.Services.AddScoped<Semou.Api.Features.Discounts.DiscountService>();
builder.Services.AddScoped<Semou.Api.Features.Books.DocBookService>();
builder.Services.AddScoped<Semou.Api.Features.Control.ControlService>();
builder.Services.AddScoped<Semou.Api.Features.Reports.AccountReportsService>();
builder.Services.AddScoped<Semou.Api.Features.Reports.SalesReportsService>();
builder.Services.AddScoped<Semou.Api.Features.Requests.RequestsService>();
builder.Services.AddScoped<Semou.Api.Features.Settings.UiConfigService>();
builder.Services.AddScoped<Semou.Api.Features.Students.StudentListsService>();
builder.Services.AddScoped<Semou.Api.Features.Documents.DocumentSealService>();
builder.Services.AddScoped<Semou.Api.Features.Registration.RegistrationService>();
builder.Services.AddScoped<Semou.Api.Features.Pricing.PricingService>();
builder.Services.AddScoped<Semou.Api.Features.Registration.AttachmentService>();
builder.Services.AddScoped<Semou.Api.Features.Sales.SalesPoster>();
builder.Services.AddScoped<Semou.Api.Features.Sales.PayInvoiceService>();
builder.Services.AddHttpClient("archive-upload", c => c.Timeout = TimeSpan.FromSeconds(90));
builder.Services.AddScoped<Semou.Api.Features.Screens.ScreenService>();

// ---------- Website link (alsmow.com): study requests in, trainee portal data out ----------
builder.Services.Configure<Semou.Api.Features.Website.WebsiteOptions>(cfg.GetSection("Website"));
// The test copy never talks to the real website: no automatic pull, no manual "تحديث الآن", no portal push.
// Exception for local development: App:TestSync=true lets it sync with a site on localhost only.
var websiteUrl = cfg["Website:BaseUrl"] ?? "";
var testSync = testMode && cfg.GetValue("App:TestSync", false)
    && Uri.TryCreate(websiteUrl, UriKind.Absolute, out var siteUri) && siteUri.IsLoopback;
if (testMode && !testSync) builder.Services.PostConfigure<Semou.Api.Features.Website.WebsiteOptions>(o => o.Enabled = false);
builder.Services.AddHttpClient<Semou.Api.Features.Website.WebsiteClient>(c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddScoped<Semou.Api.Features.Website.WebsiteSyncService>();
if (!testMode || testSync) builder.Services.AddHostedService<Semou.Api.Features.Website.WebsiteSyncWorker>();

// ---------- Auth ----------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Brute-force protection on login (passwords are stored in plain text in User_Info by the desktop app).
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.OnRejected = (ctx, ct) => new ValueTask(ctx.HttpContext.Response.WriteAsJsonAsync(
        new { message = "محاولات كثيرة، يرجى الانتظار دقيقة ثم المحاولة مرة أخرى" }, ct));
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
    o.AddPolicy("verify", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1) }));
});

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(cfg.GetSection("Cors:Origins").Get<string[]>() ?? [])
    .AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("database");

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment()) app.UseHsts();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Lets the UI show the "test copy" banner.
app.MapGet("/api/app/info", () => new { testMode, database = testMode ? dbName : null });

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = (ctx, r) => ctx.Response.WriteAsJsonAsync(new { status = r.Status.ToString() })
});

if (app.Environment.IsDevelopment())
    app.MapGet("/api/dev/screens-check", (Db db, Semou.Api.Features.Screens.ProcCatalog procs) =>
        Semou.Api.Features.Screens.ScreenSelfCheck.RunAsync(db, procs));

// In production the built React app (web -> api/wwwroot) is served from here.
// index.html is never cached (it names the current build's files); hashed /assets files are cached for a year.
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var h = ctx.Context.Response.Headers;
        if (ctx.File.Name.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) h.CacheControl = "no-cache, no-store, must-revalidate";
        else if (ctx.Context.Request.Path.StartsWithSegments("/assets")) h.CacheControl = "public, max-age=31536000, immutable";
        else if (ctx.Context.Request.Path.StartsWithSegments("/fonts")) h.CacheControl = "public, max-age=2592000";
    },
});
app.MapControllers();
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate",
});

app.Run();

internal sealed class DbHealthCheck(Db db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await using var c = await db.OpenAsync(ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database unreachable", ex);
        }
    }
}
