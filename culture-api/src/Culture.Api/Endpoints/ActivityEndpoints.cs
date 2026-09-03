using Culture.Application.Activities.CreateActivity;
using FluentValidation;

namespace Culture.Api.Endpoints;

public static class ActivityEndpoints
{
    public static RouteGroupBuilder MapActivityEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/admin/activities")
            .WithTags("Activities")
            .RequireAuthorization("CanManageActivities");

        group.MapGet("/", () =>
        {
            var rows = new[]
            {
                new ActivitySummaryResponse(Guid.NewGuid(), "Experience Check-in", "Published", DateOnly.FromDateTime(DateTime.UtcNow), 12, 8),
                new ActivitySummaryResponse(Guid.NewGuid(), "Soulver Survey", "Draft", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), 6, 0),
            };

            return Results.Ok(rows);
        });

        group.MapPost("/", async (
            CreateActivityCommand command,
            IValidator<CreateActivityCommand> validator,
            CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            return Results.Created($"/api/admin/activities/{Guid.NewGuid()}", new { id = Guid.NewGuid() });
        });

        return group;
    }
}

public sealed record ActivitySummaryResponse(
    Guid Id,
    string Title,
    string Status,
    DateOnly ScheduledDate,
    int AssignedBuddies,
    int CompletedSurveys);
