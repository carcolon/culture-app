using FluentValidation;

namespace Culture.Application.Activities.CreateActivity;

public sealed class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(180);
        RuleFor(x => x.SurveyTemplateId).NotEmpty();
    }
}
