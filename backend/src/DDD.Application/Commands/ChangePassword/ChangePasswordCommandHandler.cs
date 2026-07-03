using DDD.Domain.UserAggregate.Interfaces;
using DDD.Domain.UserAggregate.Services;
using MediatR;

namespace DDD.Application.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"用户 {request.UserId} 不存在");
        }

        if (!_passwordHasher.Verify(request.OldPassword, user.Password.Hash, user.Password.Salt))
        {
            throw new InvalidOperationException("原密码不正确");
        }

        var newSalt = _passwordHasher.GenerateSalt();
        var newHash = _passwordHasher.Hash(request.NewPassword, newSalt);

        user.ChangePassword(newHash, newSalt);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}