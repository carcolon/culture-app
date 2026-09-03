using Culture.Application.Identity;
using Culture.Domain.Identity;
using Culture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Culture.Infrastructure.Identity;

public sealed class AdminAuthenticationService(
    CultureDbContext dbContext,
    PasswordHasher<AdminUser> passwordHasher,
    IOptions<BuddyAuthenticationOptions> options) : IAdminAuthenticationService
{
    public async Task<AdminLoginResult> LoginLocalAsync(string email, string password, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string normalizedEmail = email.Trim().ToUpperInvariant();

        AdminUser? admin = await dbContext.AdminUsers
            .SingleOrDefaultAsync(x => x.Email.ToUpper() == normalizedEmail, cancellationToken);

        if (admin is null || admin.Status is not UserStatus.Active || string.IsNullOrWhiteSpace(admin.PasswordHash))
        {
            return AdminLoginResult.Failed();
        }

        if (admin.IsLockedOut(now))
        {
            return AdminLoginResult.LockedOut();
        }

        PasswordVerificationResult verification = passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
        if (verification is PasswordVerificationResult.Failed)
        {
            BuddyAuthenticationOptions lockout = options.Value;
            admin.RecordFailedLogin(now, lockout.MaxFailedAccessAttempts, TimeSpan.FromMinutes(lockout.LockoutMinutes));
            await dbContext.SaveChangesAsync(cancellationToken);
            return admin.IsLockedOut(now) ? AdminLoginResult.LockedOut() : AdminLoginResult.Failed();
        }

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
        {
            admin.SetPasswordHash(passwordHasher.HashPassword(admin, password));
        }

        admin.RecordSuccessfulLogin(now);
        await dbContext.SaveChangesAsync(cancellationToken);

        return AdminLoginResult.Success(new AuthenticatedAdmin(admin.Id, admin.Email, admin.DisplayName, admin.Role.ToString()));
    }
}
