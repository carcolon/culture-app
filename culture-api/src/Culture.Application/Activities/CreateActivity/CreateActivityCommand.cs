namespace Culture.Application.Activities.CreateActivity;

public sealed record CreateActivityCommand(
    string Title,
    string Description,
    DateOnly ScheduledDate,
    string Location,
    Guid SurveyTemplateId);
