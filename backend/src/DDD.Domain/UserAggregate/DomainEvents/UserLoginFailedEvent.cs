using DDD.Domain.Common;

namespace DDD.Domain.UserAggregate.DomainEvents;

public sealed record UserLoginFailedEvent(
    string Email,
    string IpAddress,
    string Reason,
    DateTime OccurredOn) : IDomainEvent
{
    public static UserLoginFailedEvent Create(string email, string ipAddress, string reason)
        => new(email, ipAddress, reason, DateTime.UtcNow);
}