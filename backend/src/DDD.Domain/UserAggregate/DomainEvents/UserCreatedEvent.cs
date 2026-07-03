using DDD.Domain.Common;

namespace DDD.Domain.UserAggregate.DomainEvents;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Email,
    string UserName,
    DateTime OccurredOn) : IDomainEvent
{
    public static UserCreatedEvent Create(Guid userId, string email, string userName)
        => new(userId, email, userName, DateTime.UtcNow);
}