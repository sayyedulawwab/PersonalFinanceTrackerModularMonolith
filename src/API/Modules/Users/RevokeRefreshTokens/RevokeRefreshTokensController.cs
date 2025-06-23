using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Users.RevokeRefreshTokens;

namespace API.Modules.Users.RevokeRefreshTokens;
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/users/userId:guid/refresh-tokens")]
[ApiController]
public class RevokeRefreshTokensController() : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpDelete]
    public async Task<IActionResult> RevokeRefreshTokens(
        ICommandHandler<RevokeRefreshTokensCommand, Guid> handler,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokensCommand(userId);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}