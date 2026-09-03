using Culture.SharedKernel;

namespace Culture.Domain.Surveys;

public sealed class SurveyTemplate : Entity
{
    private readonly List<Question> _questions = [];

    private SurveyTemplate()
    {
    }

    public SurveyTemplate(string name, int version)
    {
        Name = name;
        Version = version;
        Status = SurveyTemplateStatus.Draft;
    }

    public string Name { get; private set; } = string.Empty;

    public int Version { get; private set; }

    public SurveyTemplateStatus Status { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
}
