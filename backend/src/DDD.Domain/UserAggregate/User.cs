using DDD.Domain.Common;
using DDD.Domain.UserAggregate.DomainEvents;
using DDD.Domain.UserAggregate.ValueObjects;

namespace DDD.Domain.UserAggregate;

public sealed class User : AggregateRoot
{
    public string UserName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PasswordHash Password { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsLocked { get; private set; }
    public int LoginFailedCount { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private const int MaxLoginFailedCount = 5;

    private User() { }

    private User(
        string userName,
        Email email,
        PasswordHash password,
        UserRole role)
    {
        UserName = userName;
        Email = email;
        Password = password;
        Role = role;
        IsLocked = false;
        LoginFailedCount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(
        string userName,
        string email,
        string passwordHash,
        string salt,
        UserRole role = UserRole.User)
    {
        var emailVo = Email.Create(email);
        var passwordVo = PasswordHash.Create(passwordHash, salt);

        var user = new User(userName, emailVo, passwordVo, role);

        user.AddDomainEvent(UserCreatedEvent.Create(user.Id, email, userName));

        return user;
    }

    public void Login(string passwordHash, string ipAddress)
    {
        if (IsLocked)
        {
            AddDomainEvent(UserLoginFailedEvent.Create(Email.Value, ipAddress, "账户已被锁定"));
            throw new InvalidOperationException("账户已被锁定，请联系管理员");
        }

        if (Password.Hash != passwordHash)
        {
            LoginFailedCount++;
            if (LoginFailedCount >= MaxLoginFailedCount)
            {
                IsLocked = true;
                AddDomainEvent(UserLoginFailedEvent.Create(Email.Value, ipAddress, "密码错误次数过多，账户已锁定"));
                throw new InvalidOperationException("密码错误次数过多，账户已锁定");
            }

            AddDomainEvent(UserLoginFailedEvent.Create(Email.Value, ipAddress, "密码错误"));
            throw new InvalidOperationException("邮箱或密码错误");
        }

        LoginFailedCount = 0;
        LastLoginAt = DateTime.UtcNow;

        AddDomainEvent(UserLoggedInEvent.Create(Id, Email.Value, ipAddress));
    }

    public void UpdateProfile(string userName, string email)
    {
        UserName = userName;
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash, string newSalt)
    {
        Password = PasswordHash.Create(newPasswordHash, newSalt);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unlock()
    {
        IsLocked = false;
        LoginFailedCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Lock()
    {
        IsLocked = true;
        UpdatedAt = DateTime.UtcNow;
    }
}