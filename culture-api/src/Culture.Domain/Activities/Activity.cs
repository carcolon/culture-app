using Culture.SharedKernel;

namespace Culture.Domain.Activities;

public sealed class Activity : Entity
{
    private readonly List<ActivityAssignment> _assignments = [];

    private Activity()
    {
    }

    public Activity(string title, string description, DateOnly scheduledDate, string location, Guid surveyTemplateId, Guid createdBy)
    {
        Title = title;
        Description = description;
        ScheduledDate = scheduledDate;
        Location = location;
        SurveyTemplateId = surveyTemplateId;
        CreatedBy = createdBy;
        Status = ActivityStatus.Draft;

        Raise(new ActivityCreatedDomainEvent(Id, CreatedBy));
    }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateOnly ScheduledDate { get; private set; }

    public string Location { get; private set; } = string.Empty;

    public ActivityStatus Status { get; private set; }

    public Guid SurveyTemplateId { get; private set; }

    public Guid CreatedBy { get; private set; }

    public IReadOnlyCollection<ActivityAssignment> Assignments => _assignments.AsReadOnly();

    public void Publish()
    {
        if (Status is not ActivityStatus.Draft)
        {
            throw new InvalidOperationException("Only draft activities can be published.");
        }

        Status = ActivityStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AssignBuddy(Guid buddyId)
    {
        if (_assignments.Exists(x => x.BuddyId == buddyId))
        {
            return;
        }

        _assignments.Add(new ActivityAssignment(Id, buddyId));
        Raise(new BuddyAssignedToActivityDomainEvent(Id, buddyId));
    }
}
