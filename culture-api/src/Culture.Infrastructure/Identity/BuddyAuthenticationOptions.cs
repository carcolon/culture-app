namespace Culture.Infrastructure.Identity;

public sealed class BuddyAuthenticationOptions
{
    public int MaxFailedAccessAttempts { get; init; } = 5;

    public int LockoutMinutes { get; init; } = 15;
}
