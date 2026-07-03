using DDD.Domain.UserAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDD.Application.DomainEventHandlers;

public sealed class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[领域事件] 用户创建成功 | UserId: {UserId} | Email: {Email} | UserName: {UserName} | 时间: {OccurredOn}",
            notification.UserId, notification.Email, notification.UserName, notification.OccurredOn);

        return Task.CompletedTask;
    }
}