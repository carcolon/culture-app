using Culture.SharedKernel;

namespace Culture.Domain.Surveys;

public sealed class SurveySession : Entity
{
    private readonly List<SurveyAnswer> _answers = [];

    private SurveySession()
    {
    }

    public SurveySession(Guid activityId, Guid buddyId, Guid surveyTemplateId)
    {
        ActivityId = activityId;
        BuddyId = buddyId;
        SurveyTemplateId = surveyTemplateId;
        StartedAt = DateTimeOffset.UtcNow;
        Status = SurveySessionStatus.Started;
    }

    public Guid ActivityId { get; private set; }

    public Guid BuddyId { get; private set; }

    public Guid SurveyTemplateId { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public SurveySessionStatus Status { get; private set; }

    public IReadOnlyCollection<SurveyAnswer> Answers => _answers.AsReadOnly();
}
