using DDD.Domain.UserAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDD.Application.DomainEventHandlers;

public sealed class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
{
    private readonly ILogger<UserLoggedInEventHandler> _logger;

    public UserLoggedInEventHandler(ILogger<UserLoggedInEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[领域事件] 用户登录成功 | UserId: {UserId} | Email: {Email} | IP: {IpAddress} | 时间: {OccurredOn}",
            notification.UserId, notification.Email, notification.IpAddress, notification.OccurredOn);

        return Task.CompletedTask;
    }
}