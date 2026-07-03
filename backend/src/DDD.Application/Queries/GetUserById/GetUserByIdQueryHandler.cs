using AutoMapper;
using DDD.Application.DTOs;
using DDD.Domain.UserAggregate.Interfaces;
using MediatR;

namespace DDD.Application.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"用户 {request.Id} 不存在");
        }

        return _mapper.Map<UserDto>(user);
    }
}