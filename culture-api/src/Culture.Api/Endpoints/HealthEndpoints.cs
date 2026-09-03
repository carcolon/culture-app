namespace Culture.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            service = "culture-api",
            status = "healthy",
            utc = DateTimeOffset.UtcNow,
        }))
        .AllowAnonymous()
        .WithTags("Health");

        return app;
    }
}
