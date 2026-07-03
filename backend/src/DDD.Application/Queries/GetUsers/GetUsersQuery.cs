using DDD.Application.DTOs;
using MediatR;

namespace DDD.Application.Queries.GetUsers;

public sealed record GetUsersQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<UserDto>>;