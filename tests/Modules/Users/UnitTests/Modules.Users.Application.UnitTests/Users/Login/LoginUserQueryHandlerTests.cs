using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Application.Users.Login;
using Modules.Users.Domain.Users;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Users.Login;
public class LoginUserQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();

    [Fact]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        _userRepository.GetByEmail(Arg.Any<string>()).Returns((User?)null);

        var handler = new LoginUserQueryHandler(
            _userRepository,
            _refreshTokenRepository,
            _unitOfWork,
            _passwordHasher,
            _tokenProvider
        );

        Result<TokenResponse> result = await handler.Handle(
            new LoginUserQuery("notfound@example.com", "any"),
            CancellationToken.None
        );

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPasswordInvalid()
    {
        var user = User.Create("John", "Doe", "john@example.com", "hashed", DateTime.UtcNow);

        _userRepository.GetByEmail("john@example.com").Returns(user);
        _passwordHasher.Verify("wrong", "hashed").Returns(false);

        var handler = new LoginUserQueryHandler(
            _userRepository,
            _refreshTokenRepository,
            _unitOfWork,
            _passwordHasher,
            _tokenProvider
        );

        Result<TokenResponse> result = await handler.Handle(
            new LoginUserQuery("john@example.com", "wrong"),
            CancellationToken.None
        );

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenCredentialsAreValid()
    {
        var user = User.Create("John", "Doe", "john@example.com", "hashed", DateTime.UtcNow);
        _userRepository.GetByEmail("john@example.com").Returns(user);
        _passwordHasher.Verify("password", "hashed").Returns(true);
        _tokenProvider.Create(user).Returns("access-token");
        _tokenProvider.GenerateRefreshToken().Returns("refresh-token");

        var handler = new LoginUserQueryHandler(
            _userRepository,
            _refreshTokenRepository,
            _unitOfWork,
            _passwordHasher,
            _tokenProvider
        );

        Result<TokenResponse> result = await handler.Handle(
            new LoginUserQuery("john@example.com", "password"),
            CancellationToken.None
        );

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Value.AccessToken);
        Assert.Equal("refresh-token", result.Value.RefreshToken);

        _refreshTokenRepository.Received(1).Add(Arg.Any<RefreshToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
