using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Users.Login;

namespace API.Modules.Users.Login;
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/auth/login")]
[ApiController]
public class LoginController() : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpPost]
    public async Task<IActionResult> Login(
        IQueryHandler<LoginUserQuery, TokenResponse> handler,
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);

        Result<TokenResponse> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [MapToApiVersion(1)]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> LoginWithRefreshToken(
        IQueryHandler<LoginUserWithRefreshTokenQuery, TokenResponse> handler,
        LoginWithRefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserWithRefreshTokenQuery(request.RefreshToken);

        Result<TokenResponse> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}
