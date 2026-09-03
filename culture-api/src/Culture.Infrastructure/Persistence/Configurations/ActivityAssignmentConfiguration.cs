using Culture.Domain.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Culture.Infrastructure.Persistence.Configurations;

public sealed class ActivityAssignmentConfiguration : IEntityTypeConfiguration<ActivityAssignment>
{
    public void Configure(EntityTypeBuilder<ActivityAssignment> builder)
    {
        builder.ToTable("Assignments", "activities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Ignore(x => x.DomainEvents);
        builder.HasIndex(x => new { x.ActivityId, x.BuddyId }).IsUnique();
    }
}
