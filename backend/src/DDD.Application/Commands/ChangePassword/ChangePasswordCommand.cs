using MediatR;

namespace DDD.Application.Commands.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword) : IRequest;