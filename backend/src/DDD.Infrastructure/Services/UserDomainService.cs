using DDD.Domain.UserAggregate;
using DDD.Domain.UserAggregate.Interfaces;
using DDD.Domain.UserAggregate.Services;
using DDD.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Infrastructure.Services;

[Injectable(ServiceLifetime.Scoped, typeof(IUserDomainService))]
public sealed class UserDomainService : IUserDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserDomainService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await _userRepository.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<User?> AuthenticateAsync(
        string email, string password, string ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var passwordHash = _passwordHasher.Hash(password, user.Password.Salt);

        user.Login(passwordHash, ipAddress);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return user;
    }
}