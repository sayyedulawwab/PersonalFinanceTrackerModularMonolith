using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Users.Register;

namespace API.Modules.Users.Register;
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/auth/register")]
[ApiController]
public class RegisterControllerr() : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpPost]
    public async Task<IActionResult> Register(
        ICommandHandler<RegisterUserCommand, Guid> handler,
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}
