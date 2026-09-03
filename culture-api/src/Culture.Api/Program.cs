using System.Threading.RateLimiting;
using System.Security.Claims;
using Culture.Api.Endpoints;
using Culture.Api.Security;
using Culture.Application.Activities.CreateActivity;
using Culture.Application.Identity;
using Culture.Infrastructure;
using Culture.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IValidator<CreateActivityCommand>, CreateActivityCommandValidator>();
builder.Services.AddScoped<IValidator<BuddyLoginRequest>, BuddyLoginRequestValidator>();
builder.Services.AddScoped<IValidator<AdminLoginRequest>, AdminLoginRequestValidator>();
builder.Services.AddSingleton<RequestOriginValidator>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = AuthConstants.CsrfHeaderName;
    options.Cookie.Name = AuthConstants.CsrfCookieName;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        string[] allowedOrigins = builder.Configuration.GetSection("Security:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];

        policy.WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
            .WithHeaders("Content-Type", "X-CSRF-TOKEN", "Authorization")
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    int loginPermitLimit = builder.Configuration.GetValue("Security:LoginRateLimit:PermitLimit", 8);
    int loginWindowMinutes = builder.Configuration.GetValue("Security:LoginRateLimit:WindowMinutes", 5);

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 120,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
            }));

    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            $"{context.Connection.RemoteIpAddress}:{context.Request.Headers.UserAgent}",
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = loginPermitLimit,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(loginWindowMinutes),
            }));
});

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = AuthConstants.SessionScheme;
    options.DefaultChallengeScheme = AuthConstants.SessionScheme;
    options.DefaultSignInScheme = AuthConstants.SessionScheme;
});

authenticationBuilder.AddCookie(AuthConstants.SessionScheme, options =>
{
    options.Cookie.Name = AuthConstants.SessionCookieName;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        },
    };
});

if (!string.IsNullOrWhiteSpace(builder.Configuration["AzureAd:ClientId"]))
{
    authenticationBuilder.AddMicrosoftIdentityWebApp(
        builder.Configuration.GetSection("AzureAd"),
        openIdConnectScheme: AuthConstants.AdminEntraScheme,
        cookieScheme: null);
}
else
{
    authenticationBuilder.AddJwtBearer(AuthConstants.AdminEntraScheme, options =>
    {
        options.Authority = "https://login.microsoftonline.com/common/v2.0";
        options.Audience = "culture-api-local-placeholder";
        options.TokenValidationParameters.ValidateIssuer = false;
    });
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BuddyOnly", policy => policy.RequireClaim("role", "Buddy"));
    options.AddPolicy("CanManageActivities", policy =>
    {
        policy.AddAuthenticationSchemes(AuthConstants.SessionScheme, AuthConstants.AdminEntraScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "activities.manage") ||
            context.User.HasClaim("roles", "Culture.Admin") ||
            context.User.HasClaim("roles", "Culture.HRAdmin") ||
            context.User.HasClaim(ClaimTypes.Role, "SuperAdmin"));
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("culture-api"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

var app = builder.Build();

await DevelopmentDataSeeder.SeedAsync(app.Services, app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSecurityHeaders();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseStrictOriginValidation();
app.UseCsrfProtection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapBuddyEndpoints();
app.MapActivityEndpoints();

app.Run();

public partial class Program;

internal static class SecurityHeadersMiddleware
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            IHeaderDictionary headers = context.Response.Headers;
            headers.TryAdd("X-Content-Type-Options", "nosniff");
            headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
            headers.TryAdd("X-Frame-Options", "DENY");
            headers.TryAdd("Cross-Origin-Opener-Policy", "same-origin");
            headers.TryAdd("Cross-Origin-Resource-Policy", "same-origin");
            headers.TryAdd("Permissions-Policy", "accelerometer=(), autoplay=(), camera=(), encrypted-media=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
            headers.TryAdd("Content-Security-Policy", "default-src 'self'; connect-src 'self' https://login.microsoftonline.com; img-src 'self' data:; style-src 'self'; script-src 'self'; frame-ancestors 'none'; object-src 'none'; base-uri 'self'; form-action 'self' https://login.microsoftonline.com; upgrade-insecure-requests");
            await next();
        });
    }
}
