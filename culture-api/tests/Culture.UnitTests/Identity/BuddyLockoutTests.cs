using Culture.Domain.Identity;

namespace Culture.UnitTests.Identity;

public sealed class BuddyLockoutTests
{
    [Fact]
    public void RecordFailedLogin_Should_Lock_User_When_Max_Attempts_Is_Reached()
    {
        var buddy = new Buddy("EMP-1", "Test Buddy", "buddy@solvoglobal.com", "hash");
        DateTimeOffset now = DateTimeOffset.UtcNow;

        buddy.RecordFailedLogin(now, maxFailedAttempts: 2, TimeSpan.FromMinutes(15));
        buddy.RecordFailedLogin(now, maxFailedAttempts: 2, TimeSpan.FromMinutes(15));

        Assert.True(buddy.IsLockedOut(now));
    }

    [Fact]
    public void RecordSuccessfulLogin_Should_Clear_Lockout_State()
    {
        var buddy = new Buddy("EMP-1", "Test Buddy", "buddy@solvoglobal.com", "hash");
        DateTimeOffset now = DateTimeOffset.UtcNow;

        buddy.RecordFailedLogin(now, maxFailedAttempts: 1, TimeSpan.FromMinutes(15));
        buddy.RecordSuccessfulLogin(now.AddMinutes(16));

        Assert.False(buddy.IsLockedOut(now.AddMinutes(16)));
        Assert.Equal(0, buddy.AccessFailedCount);
    }
}
