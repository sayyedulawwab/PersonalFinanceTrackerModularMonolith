using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.Login;

public record LoginUserQuery(string Email, string Password) : IQuery<TokenResponse>;