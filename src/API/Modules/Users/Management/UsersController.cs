using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Auth.Register;
using Modules.Users.Application.Users;
using Modules.Users.Application.Users.GetAll;
using Modules.Users.Application.Users.GetById;
using Modules.Users.Application.Users.Update;

namespace API.Modules.Users.Management;
[ApiController]
[Route("api/v{v:apiVersion}/users")]
[ApiVersion(1)]
public class UsersController : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        IQueryHandler<GetAllUserQuery, IReadOnlyList<UserResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllUserQuery();

        Result<IReadOnlyList<UserResponse>> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }


    [MapToApiVersion(1)]
    [HttpGet("userId:guid")]
    public async Task<IActionResult> GetById(
        IQueryHandler<GetByIdUserQuery, UserResponse> handler,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdUserQuery(userId);

        Result<UserResponse> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [MapToApiVersion(1)]
    [HttpPut("userId:guid")]
    public async Task<IActionResult> Update(
        ICommandHandler<UpdateUserCommand, Guid> handler,
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            userId,
            request.FirstName,
            request.LastName);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}
