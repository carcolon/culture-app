using Culture.SharedKernel;

namespace Culture.Domain.Identity;

public sealed class Buddy : Entity
{
    private Buddy()
    {
    }

    public Buddy(string employeeCode, string fullName, string email, string passwordHash)
    {
        EmployeeCode = employeeCode;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Status = UserStatus.Active;
    }

    public string EmployeeCode { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

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
