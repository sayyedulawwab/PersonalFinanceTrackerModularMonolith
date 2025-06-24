using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Auth.Login;
public record LoginUserWithRefreshTokenQuery(string RefreshToken) : IQuery<TokenResponse>;