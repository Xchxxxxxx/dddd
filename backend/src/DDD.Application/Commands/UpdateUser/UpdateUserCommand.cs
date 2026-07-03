using DDD.Application.DTOs;
using MediatR;

namespace DDD.Application.Commands.UpdateUser;

public sealed record UpdateUserCommand(Guid Id, string UserName, string Email) : IRequest<UserDto>;