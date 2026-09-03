using Culture.SharedKernel;

namespace Culture.Domain.Surveys;

public sealed class Question : Entity
{
    private Question()
    {
    }

    public Question(Guid surveyTemplateId, string text, QuestionType type, int order, bool isRequired)
    {
        SurveyTemplateId = surveyTemplateId;
        Text = text;
        Type = type;
        Order = order;
        IsRequired = isRequired;
    }

    public Guid SurveyTemplateId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public QuestionType Type { get; private set; }

    public int Order { get; private set; }

    public bool IsRequired { get; private set; }
}
