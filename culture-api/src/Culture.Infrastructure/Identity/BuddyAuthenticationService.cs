using Culture.Application.Identity;
using Culture.Domain.Identity;
using Culture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Culture.Infrastructure.Identity;

public sealed class BuddyAuthenticationService(
    CultureDbContext dbContext,
    PasswordHasher<Buddy> passwordHasher,
    IOptions<BuddyAuthenticationOptions> options) : IBuddyAuthenticationService
{
    public async Task<BuddyLoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string normalizedEmail = email.Trim().ToUpperInvariant();

        Buddy? buddy = await dbContext.Buddies
            .SingleOrDefaultAsync(x => x.Email.ToUpper() == normalizedEmail, cancellationToken);

        if (buddy is null || buddy.Status is not UserStatus.Active)
        {
            return BuddyLoginResult.Failed();
        }

        if (buddy.IsLockedOut(now))
        {
            return BuddyLoginResult.LockedOut();
        }

        PasswordVerificationResult verification = passwordHasher.VerifyHashedPassword(buddy, buddy.PasswordHash, password);
        if (verification is PasswordVerificationResult.Failed)
        {
            BuddyAuthenticationOptions lockout = options.Value;
            buddy.RecordFailedLogin(now, lockout.MaxFailedAccessAttempts, TimeSpan.FromMinutes(lockout.LockoutMinutes));
            await dbContext.SaveChangesAsync(cancellationToken);
            return buddy.IsLockedOut(now) ? BuddyLoginResult.LockedOut() : BuddyLoginResult.Failed();
        }

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
        {
            buddy.SetPasswordHash(passwordHasher.HashPassword(buddy, password));
        }

        buddy.RecordSuccessfulLogin(now);
        await dbContext.SaveChangesAsync(cancellationToken);

        return BuddyLoginResult.Success(new AuthenticatedBuddy(buddy.Id, buddy.Email, buddy.FullName));
    }
}
