namespace Culture.Api.Endpoints;

public static class BuddyEndpoints
{
    public static RouteGroupBuilder MapBuddyEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/buddy")
            .WithTags("Buddy")
            .RequireAuthorization("BuddyOnly");

        group.MapGet("/activities", () =>
        {
            var rows = new[]
            {
                new BuddyActivityResponse(Guid.NewGuid(), "Encuesta Pulse", "Centro Barranquilla", "Available", 3),
                new BuddyActivityResponse(Guid.NewGuid(), "Culture & Experience", "Sede Medellin", "InProgress", 1),
            };

            return Results.Ok(rows);
        });

        group.MapPost("/activities/{activityId:guid}/check-in", (Guid activityId) =>
            Results.Ok(new { activityId, status = "CheckedIn" }));

        group.MapPost("/activities/{activityId:guid}/check-out", (Guid activityId) =>
            Results.Ok(new { activityId, status = "CheckedOut" }));

        return group;
    }
}

public sealed record BuddyActivityResponse(
    Guid Id,
    string Title,
    string Location,
    string Status,
    int PendingQuestions);
