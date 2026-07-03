using DDD.Application.Commands.ChangePassword;
using DDD.Application.Commands.CreateUser;
using DDD.Application.Commands.DeleteUser;
using DDD.Application.Commands.ToggleUserLock;
using DDD.Application.Commands.UpdateUser;
using DDD.Application.DTOs;
using DDD.Application.Queries.GetUserById;
using DDD.Application.Queries.GetUsers;
using DDD.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDD.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResult<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return Ok(ApiResult<PagedResult<UserDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        return Ok(ApiResult<UserDto>.Ok(result));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResult<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.UserName, request.Email, request.Password, request.Role);
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetUser), new { id = result.Id },
            ApiResult<UserDto>.Ok(result, "用户创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(id, request.UserName, request.Email);
        var result = await _mediator.Send(command);

        return Ok(ApiResult<UserDto>.Ok(result, "用户更新成功"));
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<ActionResult<ApiResult>> ChangePassword(
        Guid id, [FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand(id, request.OldPassword, request.NewPassword);
        await _mediator.Send(command);

        return Ok(ApiResult.Ok("密码修改成功"));
    }

    [HttpPost("{id:guid}/lock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResult>> LockUser(Guid id)
    {
        var command = new ToggleUserLockCommand(id, true);
        await _mediator.Send(command);

        return Ok(ApiResult.Ok("用户已锁定"));
    }

    [HttpPost("{id:guid}/unlock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResult>> UnlockUser(Guid id)
    {
        var command = new ToggleUserLockCommand(id, false);
        await _mediator.Send(command);

        return Ok(ApiResult.Ok("用户已解锁"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResult>> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        await _mediator.Send(command);

        return Ok(ApiResult.Ok("用户已删除"));
    }
}