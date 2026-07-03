using DDD.Application.DTOs;
using MediatR;

namespace DDD.Application.Commands.CreateUser;

public sealed record CreateUserCommand(
    string UserName,
    string Email,
    string Password,
    string Role) : IRequest<UserDto>;