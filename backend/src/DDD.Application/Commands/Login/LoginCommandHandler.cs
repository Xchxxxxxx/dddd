using AutoMapper;
using DDD.Application.DTOs;
using DDD.Domain.UserAggregate.Services;
using DDD.Infrastructure.Services;
using MediatR;

namespace DDD.Application.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserDomainService _userDomainService;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserDomainService userDomainService,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userDomainService = userDomainService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userDomainService.AuthenticateAsync(
            request.Email, request.Password, request.IpAddress, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("邮箱或密码错误");
        }

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(
            user.Id, user.Email.Value, user.Role.ToString());

        var userDto = _mapper.Map<UserDto>(user);

        return new LoginResponse(accessToken, refreshToken, userDto);
    }
}