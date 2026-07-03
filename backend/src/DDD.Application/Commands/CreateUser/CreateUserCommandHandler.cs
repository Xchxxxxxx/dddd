using AutoMapper;
using DDD.Application.DTOs;
using DDD.Domain.UserAggregate;
using DDD.Domain.UserAggregate.Interfaces;
using DDD.Domain.UserAggregate.Services;
using MediatR;

namespace DDD.Application.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserDomainService _userDomainService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserDomainService userDomainService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userDomainService = userDomainService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await _userDomainService.IsEmailUniqueAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("该邮箱已被注册");
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new ArgumentException("无效的角色类型");
        }

        var salt = _passwordHasher.GenerateSalt();
        var passwordHash = _passwordHasher.Hash(request.Password, salt);

        var user = User.Create(request.UserName, request.Email, passwordHash, salt, role);

        await _userRepository.AddAsync(user, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}