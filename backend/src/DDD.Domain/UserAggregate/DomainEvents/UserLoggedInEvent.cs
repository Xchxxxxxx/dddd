using DDD.Domain.Common;

namespace DDD.Domain.UserAggregate.DomainEvents;

public sealed record UserLoggedInEvent(
    Guid UserId,
    string Email,
    string IpAddress,
    DateTime OccurredOn) : IDomainEvent
{
    public static UserLoggedInEvent Create(Guid userId, string email, string ipAddress)
        => new(userId, email, ipAddress, DateTime.UtcNow);
}