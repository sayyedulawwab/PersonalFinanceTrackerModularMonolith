using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Auth.RevokeRefreshTokens;
public record RevokeRefreshTokensCommand(Guid UserId) : ICommand<Guid>;