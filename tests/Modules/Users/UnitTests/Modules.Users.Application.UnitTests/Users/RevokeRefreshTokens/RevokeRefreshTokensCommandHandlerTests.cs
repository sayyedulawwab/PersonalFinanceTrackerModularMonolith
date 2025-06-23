using Common.Domain.Abstractions;
using Modules.Users.Application.Users.RevokeRefreshTokens;
using Modules.Users.Domain.Users;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Users.RevokeRefreshTokens;
public class RevokeRefreshTokensCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

    [Fact]
    public async Task Handle_ShouldDeleteTokensAndReturnUserId()
    {
        var userId = Guid.NewGuid();

        var handler = new RevokeRefreshTokensCommandHandler(_refreshTokenRepository);

        Result<Guid> result = await handler.Handle(new RevokeRefreshTokensCommand(userId), CancellationToken.None);

        await _refreshTokenRepository.Received(1).DeleteByUserIdAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }
}
