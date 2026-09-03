using Microsoft.AspNetCore.Antiforgery;

namespace Culture.Api.Security;

public static class SecurityMiddlewareExtensions
{
    private static readonly HashSet<string> UnsafeMethods = ["POST", "PUT", "PATCH", "DELETE"];

    public static IApplicationBuilder UseStrictOriginValidation(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (UnsafeMethods.Contains(context.Request.Method))
            {
                var validator = context.RequestServices.GetRequiredService<RequestOriginValidator>();
                if (!validator.IsAllowed(context.Request))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid request origin." });
                    return;
                }
            }

            await next();
        });
    }

    public static IApplicationBuilder UseCsrfProtection(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (UnsafeMethods.Contains(context.Request.Method))
            {
                IAntiforgery antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

                try
                {
                    await antiforgery.ValidateRequestAsync(context);
                }
                catch (AntiforgeryValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid CSRF token." });
                    return;
                }
            }

            await next();
        });
    }
}
