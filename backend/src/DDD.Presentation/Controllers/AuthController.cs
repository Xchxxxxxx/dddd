using DDD.Application.Commands.Login;
using DDD.Application.DTOs;
using DDD.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DDD.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var command = new LoginCommand(request.Email, request.Password, ipAddress);
        var result = await _mediator.Send(command);

        return Ok(ApiResult<LoginResponse>.Ok(result, "登录成功"));
    }
}