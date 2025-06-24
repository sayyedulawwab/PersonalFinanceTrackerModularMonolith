using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Auth.RevokeRefreshTokens;
public sealed class RevokeRefreshTokensCommandHandler(IRefreshTokenRepository refreshTokenRepository) : ICommandHandler<RevokeRefreshTokensCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RevokeRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.DeleteByUserIdAsync(request.UserId);

        return request.UserId;
    }
}