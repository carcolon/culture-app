using Culture.SharedKernel;

namespace Culture.Domain.Activities;

public sealed class ActivityAssignment : Entity
{
    private ActivityAssignment()
    {
    }

    internal ActivityAssignment(Guid activityId, Guid buddyId)
    {
        ActivityId = activityId;
        BuddyId = buddyId;
        Status = ActivityAssignmentStatus.Assigned;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    public Guid ActivityId { get; private set; }

    public Guid BuddyId { get; private set; }

    public DateTimeOffset AssignedAt { get; private set; }

    public ActivityAssignmentStatus Status { get; private set; }
}
