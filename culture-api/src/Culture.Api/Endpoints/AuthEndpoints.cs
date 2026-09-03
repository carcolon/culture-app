using System.Security.Claims;
using Culture.Api.Security;
using Culture.Application.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Culture.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapGet("/csrf", (HttpContext context, IAntiforgery antiforgery) =>
        {
            AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
            return Results.Ok(new CsrfTokenResponse(tokens.RequestToken ?? string.Empty));
        })
        .AllowAnonymous();

        group.MapPost("/buddy/login", async (
            BuddyLoginRequest request,
            IBuddyAuthenticationService authenticationService,
            IValidator<BuddyLoginRequest> validator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            BuddyLoginResult result = await authenticationService.LoginAsync(request.Email, request.Password, cancellationToken);
            if (result.IsLockedOut)
            {
                return Results.Json(new { error = "Account locked. Try again later." }, statusCode: StatusCodes.Status423Locked);
            }

            if (!result.Succeeded || result.Buddy is null)
            {
                return Results.Unauthorized();
            }

            ClaimsPrincipal principal = CreateBuddyPrincipal(result.Buddy);
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            };

            await httpContext.SignInAsync(AuthConstants.SessionScheme, principal, properties);

            return Results.Ok(new AuthenticatedUserResponse(result.Buddy.Id, result.Buddy.Email, result.Buddy.FullName, "Buddy"));
        })
        .AllowAnonymous()
        .RequireRateLimiting("login");

        if (app.ServiceProvider.GetRequiredService<IHostEnvironment>().IsEnvironment("Local"))
        {
            group.MapPost("/admin/local-login", async (
                AdminLoginRequest request,
                IAdminAuthenticationService authenticationService,
                IValidator<AdminLoginRequest> validator,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var validation = await validator.ValidateAsync(request, cancellationToken);
                if (!validation.IsValid)
                {
                    return Results.ValidationProblem(validation.ToDictionary());
                }

                AdminLoginResult result = await authenticationService.LoginLocalAsync(request.Email, request.Password, cancellationToken);
                if (result.IsLockedOut)
                {
                    return Results.Json(new { error = "Account locked. Try again later." }, statusCode: StatusCodes.Status423Locked);
                }

                if (!result.Succeeded || result.Admin is null)
                {
                    return Results.Unauthorized();
                }

                ClaimsPrincipal principal = CreateAdminPrincipal(result.Admin);
                var properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                };

                await httpContext.SignInAsync(AuthConstants.SessionScheme, principal, properties);

                return Results.Ok(new AuthenticatedUserResponse(result.Admin.Id, result.Admin.Email, result.Admin.DisplayName, result.Admin.Role));
            })
            .AllowAnonymous()
            .RequireRateLimiting("login");
        }

        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(AuthConstants.SessionScheme);
            return Results.NoContent();
        })
        .RequireAuthorization();

        group.MapGet("/me", [Authorize] (ClaimsPrincipal user) =>
        {
            string id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            string email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            string name = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            string role = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role") ?? string.Empty;

            return Results.Ok(new AuthenticatedUserResponse(Guid.TryParse(id, out Guid parsedId) ? parsedId : Guid.Empty, email, name, role));
        });

        group.MapGet("/admin/challenge", () => Results.Challenge(
            authenticationSchemes: [AuthConstants.AdminEntraScheme]))
        .AllowAnonymous();

        return group;
    }

    private static ClaimsPrincipal CreateBuddyPrincipal(AuthenticatedBuddy buddy)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, buddy.Id.ToString()),
            new(ClaimTypes.Email, buddy.Email),
            new(ClaimTypes.Name, buddy.FullName),
            new(ClaimTypes.Role, "Buddy"),
            new("role", "Buddy"),
        };

        var identity = new ClaimsIdentity(claims, AuthConstants.SessionScheme);
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreateAdminPrincipal(AuthenticatedAdmin admin)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Name, admin.DisplayName),
            new(ClaimTypes.Role, admin.Role),
            new("role", admin.Role),
            new("roles", "Culture.Admin"),
            new("permission", "activities.manage"),
        };

        var identity = new ClaimsIdentity(claims, AuthConstants.SessionScheme);
        return new ClaimsPrincipal(identity);
    }
}

public sealed record CsrfTokenResponse(string Token);

public sealed record AuthenticatedUserResponse(Guid Id, string Email, string Name, string Role);
