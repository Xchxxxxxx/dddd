using DDD.Application.DTOs;
using MediatR;

namespace DDD.Application.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string IpAddress) : IRequest<LoginResponse>;