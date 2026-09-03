namespace Culture.SharedKernel;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
