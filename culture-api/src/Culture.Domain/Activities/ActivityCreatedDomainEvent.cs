using Culture.SharedKernel;

namespace Culture.Domain.Activities;

public sealed record ActivityCreatedDomainEvent(Guid ActivityId, Guid CreatedBy, DateTimeOffset OccurredAt) : IDomainEvent
{
    public ActivityCreatedDomainEvent(Guid activityId, Guid createdBy)
        : this(activityId, createdBy, DateTimeOffset.UtcNow)
    {
    }
}
