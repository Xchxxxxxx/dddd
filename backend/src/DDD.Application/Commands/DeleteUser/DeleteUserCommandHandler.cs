using DDD.Domain.UserAggregate.Interfaces;
using MediatR;

namespace DDD.Application.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
    }
}