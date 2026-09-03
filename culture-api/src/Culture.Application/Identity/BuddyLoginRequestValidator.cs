using FluentValidation;

namespace Culture.Application.Identity;

public sealed class BuddyLoginRequestValidator : AbstractValidator<BuddyLoginRequest>
{
    public BuddyLoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
    }
}
