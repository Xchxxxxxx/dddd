using DDD.Application.DTOs;
using MediatR;

namespace DDD.Application.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;