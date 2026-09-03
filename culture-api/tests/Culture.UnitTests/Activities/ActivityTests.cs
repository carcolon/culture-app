using Culture.Domain.Activities;

namespace Culture.UnitTests.Activities;

public sealed class ActivityTests
{
    [Fact]
    public void AssignBuddy_Should_Not_Duplicate_Assignment()
    {
        var activity = new Activity("Pulse Check", "Weekly survey", DateOnly.FromDateTime(DateTime.UtcNow), "Barranquilla", Guid.NewGuid(), Guid.NewGuid());
        Guid buddyId = Guid.NewGuid();

        activity.AssignBuddy(buddyId);
        activity.AssignBuddy(buddyId);

        Assert.Single(activity.Assignments);
    }
}
