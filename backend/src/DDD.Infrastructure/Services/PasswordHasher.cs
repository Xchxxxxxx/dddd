using System.Security.Cryptography;
using DDD.Domain.UserAggregate.Services;
using DDD.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Infrastructure.Services;

[Injectable(ServiceLifetime.Scoped, typeof(IPasswordHasher))]
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 64;
    private const int Iterations = 100000;

    public string Hash(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        return Convert.ToBase64String(hash);
    }

    public string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        return Convert.ToBase64String(saltBytes);
    }

    public bool Verify(string password, string hash, string salt)
    {
        var computedHash = Hash(password, salt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(hash));
    }
}