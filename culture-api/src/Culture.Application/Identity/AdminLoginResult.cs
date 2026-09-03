namespace Culture.Application.Identity;

public sealed record AdminLoginResult(bool Succeeded, bool IsLockedOut, AuthenticatedAdmin? Admin, string? Error)
{
    public static AdminLoginResult Success(AuthenticatedAdmin admin) => new(true, false, admin, null);

    public static AdminLoginResult Failed() => new(false, false, null, "Invalid credentials.");

    public static AdminLoginResult LockedOut() => new(false, true, null, "Account is locked.");
}
