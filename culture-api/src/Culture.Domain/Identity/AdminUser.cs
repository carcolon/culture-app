using Culture.SharedKernel;

namespace Culture.Domain.Identity;

public sealed class AdminUser : Entity
{
    private AdminUser()
    {
    }

    public AdminUser(Guid entraObjectId, string email, string displayName, AdminRole role)
    {
        EntraObjectId = entraObjectId;
        Email = email;
        DisplayName = displayName;
        PasswordHash = string.Empty;
        Role = role;
        Status = UserStatus.Active;
    }

    public Guid EntraObjectId { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public AdminRole Role { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public int AccessFailedCount { get; private set; }

    public DateTimeOffset? LockoutEnd { get; private set; }

    public bool IsLockedOut(DateTimeOffset now) => LockoutEnd is not null && LockoutEnd > now;

    public void RecordSuccessfulLogin(DateTimeOffset now)
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        LastLoginAt = now;
        Status = UserStatus.Active;
        UpdatedAt = now;
    }

    public void RecordFailedLogin(DateTimeOffset now, int maxFailedAttempts, TimeSpan lockoutDuration)
    {
        AccessFailedCount++;

        if (AccessFailedCount >= maxFailedAttempts)
        {
            LockoutEnd = now.Add(lockoutDuration);
        }

        UpdatedAt = now;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ResetDevelopmentCredentials(string passwordHash)
    {
        PasswordHash = passwordHash;
        AccessFailedCount = 0;
        LockoutEnd = null;
        Status = UserStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
