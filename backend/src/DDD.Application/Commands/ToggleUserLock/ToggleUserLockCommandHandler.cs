using DDD.Domain.UserAggregate.Interfaces;
using MediatR;

namespace DDD.Application.Commands.ToggleUserLock;

public sealed class ToggleUserLockCommandHandler : IRequestHandler<ToggleUserLockCommand>
{
    private readonly IUserRepository _userRepository;

    public ToggleUserLockCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ToggleUserLockCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"用户 {request.UserId} 不存在");
        }

        if (request.Lock)
        {
            user.Lock();
        }
        else
        {
            user.Unlock();
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}