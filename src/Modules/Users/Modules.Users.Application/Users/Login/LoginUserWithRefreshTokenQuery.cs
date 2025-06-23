using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.Login;
public record LoginUserWithRefreshTokenQuery(string RefreshToken) : IQuery<TokenResponse>;