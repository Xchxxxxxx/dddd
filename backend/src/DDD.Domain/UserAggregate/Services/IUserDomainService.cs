namespace DDD.Domain.UserAggregate.Services;

public interface IUserDomainService
{
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateAsync(string email, string password, string ipAddress, CancellationToken cancellationToken = default);
}