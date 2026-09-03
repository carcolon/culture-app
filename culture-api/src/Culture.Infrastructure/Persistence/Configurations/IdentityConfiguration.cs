using Culture.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Culture.Infrastructure.Persistence.Configurations;

public sealed class BuddyConfiguration : IEntityTypeConfiguration<Buddy>
{
    public void Configure(EntityTypeBuilder<Buddy> builder)
    {
        builder.ToTable("Buddies", "identity");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).HasMaxLength(64).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.LockoutEnd);
        builder.Property(x => x.AccessFailedCount);
        builder.Ignore(x => x.DomainEvents);
        builder.HasIndex(x => x.EmployeeCode).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

public sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("AdminUsers", "identity");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.LockoutEnd);
        builder.Property(x => x.AccessFailedCount);
        builder.Ignore(x => x.DomainEvents);
        builder.HasIndex(x => x.EntraObjectId).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
