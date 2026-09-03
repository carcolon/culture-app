using Culture.SharedKernel;

namespace Culture.Domain.Surveys;

public sealed class SurveyAnswer : Entity
{
    private SurveyAnswer()
    {
    }

    public SurveyAnswer(Guid surveySessionId, Guid questionId, string value)
    {
        SurveySessionId = surveySessionId;
        QuestionId = questionId;
        Value = value;
    }

    public Guid SurveySessionId { get; private set; }

    public Guid QuestionId { get; private set; }

    public string Value { get; private set; } = string.Empty;
}
