using Culture.SharedKernel;

namespace Culture.Domain.Activities;

public sealed record BuddyAssignedToActivityDomainEvent(Guid ActivityId, Guid BuddyId, DateTimeOffset OccurredAt) : IDomainEvent
{
    public BuddyAssignedToActivityDomainEvent(Guid activityId, Guid buddyId)
        : this(activityId, buddyId, DateTimeOffset.UtcNow)
    {
    }
}
