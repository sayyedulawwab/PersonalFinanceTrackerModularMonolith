using Modules.Users.Domain.Users;

namespace Modules.Users.Domain.UnitTests.Users;
public class RefreshTokenTests
{
    [Fact]
    public void Create_ShouldInitializeRefreshTokenCorrectly()
    {
        // Arrange
        string token = "sometoken";
        var userId = Guid.NewGuid();
        DateTime expiresOn = DateTime.UtcNow.AddDays(7);

        // Act
        var refreshToken = RefreshToken.Create(token, userId, expiresOn);

        // Assert
        Assert.NotEqual(Guid.Empty, refreshToken.Id);
        Assert.Equal(token, refreshToken.Token);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.Equal(expiresOn, refreshToken.ExpiresOnUtc);
    }

    [Fact]
    public void Update_ShouldChangeTokenAndExpiry()
    {
        // Arrange
        var refreshToken = RefreshToken.Create("oldToken", Guid.NewGuid(), DateTime.UtcNow.AddDays(1));
        string newToken = "newToken";
        DateTime newExpiry = DateTime.UtcNow.AddDays(10);

        // Act
        var updatedToken = RefreshToken.Update(refreshToken, newToken, newExpiry);

        // Assert
        Assert.Equal(refreshToken.Id, updatedToken.Id);
        Assert.Equal(newToken, updatedToken.Token);
        Assert.Equal(newExpiry, updatedToken.ExpiresOnUtc);
    }
}
