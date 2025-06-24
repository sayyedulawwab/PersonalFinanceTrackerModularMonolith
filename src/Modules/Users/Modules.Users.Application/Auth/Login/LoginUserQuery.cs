using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Auth.Login;

public record LoginUserQuery(string Email, string Password) : IQuery<TokenResponse>;