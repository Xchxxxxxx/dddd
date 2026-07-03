using DDD.Domain.Common;

namespace DDD.Domain.UserAggregate.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public string Hash { get; }
    public string Salt { get; }

    private PasswordHash(string hash, string salt)
    {
        Hash = hash;
        Salt = salt;
    }

    public static PasswordHash Create(string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("密码哈希不能为空", nameof(hash));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("盐值不能为空", nameof(salt));

        return new PasswordHash(hash, salt);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
}