using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Application.Auth.Login;
using Modules.Users.Domain.Users;
using NSubstitute;
using System.Reflection;

namespace Modules.Users.Application.UnitTests.Auth.Login;
public class LoginUserWithRefreshTokenQueryHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();

    [Fact]
    public async Task Handle_ShouldFail_WhenTokenIsNull()
    {
        _refreshTokenRepository.GetByTokenAsync("token").Returns((RefreshToken?)null);

        var handler = new LoginUserWithRefreshTokenQueryHandler(_refreshTokenRepository, _unitOfWork, _tokenProvider);

        Result<TokenResponse> result = await handler.Handle(new LoginUserWithRefreshTokenQuery("token"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(RefreshTokenErrors.Expired, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTokenIsExpired()
    {
        var token = RefreshToken.Create("token", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-5));

        _refreshTokenRepository.GetByTokenAsync("token").Returns(token);

        var handler = new LoginUserWithRefreshTokenQueryHandler(_refreshTokenRepository, _unitOfWork, _tokenProvider);

        Result<TokenResponse> result = await handler.Handle(new LoginUserWithRefreshTokenQuery("token"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(RefreshTokenErrors.Expired, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenTokenIsValid()
    {
        var user = User.Create("John", "Doe", "john@example.com", "hash", DateTime.UtcNow);
        var refreshToken = RefreshToken.Create("token", user.Id, DateTime.UtcNow.AddMinutes(10));

        FieldInfo? field = typeof(RefreshToken).GetField($"<{nameof(RefreshToken.User)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (field == null)
        {
            throw new InvalidOperationException("Could not find backing field for 'User'");
        }

        field.SetValue(refreshToken, user);

        _refreshTokenRepository.GetByTokenAsync("token").Returns(refreshToken);
        _tokenProvider.Create(user).Returns("access-token");
        _tokenProvider.GenerateRefreshToken().Returns("new-refresh");

        var handler = new LoginUserWithRefreshTokenQueryHandler(_refreshTokenRepository, _unitOfWork, _tokenProvider);

        Result<TokenResponse> result = await handler.Handle(new LoginUserWithRefreshTokenQuery("token"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Value.AccessToken);
        Assert.Equal("new-refresh", result.Value.RefreshToken);

        _refreshTokenRepository.Received(1).Update(Arg.Any<RefreshToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
