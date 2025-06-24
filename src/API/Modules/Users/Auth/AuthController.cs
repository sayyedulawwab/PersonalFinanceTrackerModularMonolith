using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Auth.Login;
using Modules.Users.Application.Auth.Register;
using Modules.Users.Application.Auth.RevokeRefreshTokens;

namespace API.Modules.Users.Auth;

[ApiController]
[Route("api/v{v:apiVersion}/auth")]
[ApiVersion(1)]
public class AuthController : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpPost("register")]
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

    [MapToApiVersion(1)]
    [HttpPost("login")]
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

    [MapToApiVersion(1)]
    [HttpDelete("refresh-tokens")]
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
