using DDD.Domain.Common;

namespace DDD.Domain.UserAggregate.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("邮箱不能为空", nameof(email));

        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("邮箱格式不正确", nameof(email));

        return new Email(email.ToLowerInvariant().Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}