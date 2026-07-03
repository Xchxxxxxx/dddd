using AutoMapper;
using DDD.Application.DTOs;
using DDD.Domain.UserAggregate.Interfaces;
using MediatR;

namespace DDD.Application.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _userRepository.CountAsync(cancellationToken);

        var userDtos = _mapper.Map<IReadOnlyList<UserDto>>(users);

        return new PagedResult<UserDto>(userDtos, totalCount, request.Page, request.PageSize);
    }
}