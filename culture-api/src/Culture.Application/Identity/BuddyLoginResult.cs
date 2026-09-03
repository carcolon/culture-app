namespace Culture.Application.Identity;

public sealed record BuddyLoginResult(bool Succeeded, bool IsLockedOut, AuthenticatedBuddy? Buddy, string? Error)
{
    public static BuddyLoginResult Success(AuthenticatedBuddy buddy) => new(true, false, buddy, null);

    public static BuddyLoginResult Failed() => new(false, false, null, "Invalid credentials.");

    public static BuddyLoginResult LockedOut() => new(false, true, null, "Account is locked.");
}
