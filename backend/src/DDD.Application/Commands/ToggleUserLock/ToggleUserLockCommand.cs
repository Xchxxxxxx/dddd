using MediatR;

namespace DDD.Application.Commands.ToggleUserLock;

public sealed record ToggleUserLockCommand(Guid UserId, bool Lock) : IRequest;