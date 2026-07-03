using DDD.Domain.UserAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDD.Application.DomainEventHandlers;

public sealed class UserLoginFailedEventHandler : INotificationHandler<UserLoginFailedEvent>
{
    private readonly ILogger<UserLoginFailedEventHandler> _logger;

    public UserLoginFailedEventHandler(ILogger<UserLoginFailedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserLoginFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "[领域事件] 用户登录失败 | Email: {Email} | IP: {IpAddress} | 原因: {Reason} | 时间: {OccurredOn}",
            notification.Email, notification.IpAddress, notification.Reason, notification.OccurredOn);

        return Task.CompletedTask;
    }
}