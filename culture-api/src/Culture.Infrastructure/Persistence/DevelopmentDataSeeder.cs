using Culture.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Culture.Infrastructure.Persistence;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        if (!configuration.GetValue("DevelopmentSeed:Enabled", false))
        {
            return;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        CultureDbContext dbContext = scope.ServiceProvider.GetRequiredService<CultureDbContext>();
        PasswordHasher<Buddy> buddyPasswordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<Buddy>>();
        PasswordHasher<AdminUser> adminPasswordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<AdminUser>>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureLocalAdminColumnsAsync(dbContext, cancellationToken);

        string buddyEmail = configuration["DevelopmentSeed:Buddy:Email"] ?? "buddy@solvoglobal.com";
        string buddyPassword = configuration["DevelopmentSeed:Buddy:Password"] ?? "ChangeMe123!";

        Buddy? existingBuddy = await dbContext.Buddies.SingleOrDefaultAsync(x => x.Email == buddyEmail, cancellationToken);
        if (existingBuddy is not null)
        {
            existingBuddy.ResetDevelopmentCredentials(buddyPasswordHasher.HashPassword(existingBuddy, buddyPassword));
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var buddy = new Buddy("DEV-BUDDY", "Development Buddy", buddyEmail, string.Empty);
            buddy.SetPasswordHash(buddyPasswordHasher.HashPassword(buddy, buddyPassword));
            dbContext.Buddies.Add(buddy);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        string adminEmail = configuration["DevelopmentSeed:Admin:Email"] ?? "admin@solvoglobal.com";
        string adminPassword = configuration["DevelopmentSeed:Admin:Password"] ?? "ChangeMe123!";
        Guid adminEntraObjectId = Guid.Parse(configuration["DevelopmentSeed:Admin:EntraObjectId"] ?? "11111111-1111-1111-1111-111111111111");

        AdminUser? existingAdmin = await dbContext.AdminUsers.SingleOrDefaultAsync(x => x.Email == adminEmail, cancellationToken);
        if (existingAdmin is not null)
        {
            existingAdmin.ResetDevelopmentCredentials(adminPasswordHasher.HashPassword(existingAdmin, adminPassword));
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var admin = new AdminUser(adminEntraObjectId, adminEmail, "Development Admin", AdminRole.SuperAdmin);
        admin.SetPasswordHash(adminPasswordHasher.HashPassword(admin, adminPassword));
        dbContext.AdminUsers.Add(admin);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureLocalAdminColumnsAsync(CultureDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync("""
IF OBJECT_ID(N'[identity].[AdminUsers]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[identity].[AdminUsers]', N'PasswordHash') IS NULL
        ALTER TABLE [identity].[AdminUsers] ADD [PasswordHash] nvarchar(512) NOT NULL CONSTRAINT [DF_AdminUsers_PasswordHash] DEFAULT N'';
    IF COL_LENGTH(N'[identity].[AdminUsers]', N'LastLoginAt') IS NULL
        ALTER TABLE [identity].[AdminUsers] ADD [LastLoginAt] datetimeoffset NULL;
    IF COL_LENGTH(N'[identity].[AdminUsers]', N'AccessFailedCount') IS NULL
        ALTER TABLE [identity].[AdminUsers] ADD [AccessFailedCount] int NOT NULL CONSTRAINT [DF_AdminUsers_AccessFailedCount] DEFAULT 0;
    IF COL_LENGTH(N'[identity].[AdminUsers]', N'LockoutEnd') IS NULL
        ALTER TABLE [identity].[AdminUsers] ADD [LockoutEnd] datetimeoffset NULL;
END
""", cancellationToken);
    }
}
