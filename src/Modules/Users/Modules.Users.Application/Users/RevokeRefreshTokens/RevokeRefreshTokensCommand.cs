using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.RevokeRefreshTokens;
public record RevokeRefreshTokensCommand(Guid UserId) : ICommand<Guid>;