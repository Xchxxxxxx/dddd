using MediatR;

namespace DDD.Application.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest;