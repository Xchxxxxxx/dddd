namespace DDD.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string Role,
    bool IsLocked,
    DateTime? LastLoginAt,
    DateTime CreatedAt);

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    UserDto User);

public sealed record CreateUserRequest(
    string UserName,
    string Email,
    string Password,
    string Role = "User");

public sealed record UpdateUserRequest(
    string UserName,
    string Email);

public sealed record ChangePasswordRequest(
    string OldPassword,
    string NewPassword);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);