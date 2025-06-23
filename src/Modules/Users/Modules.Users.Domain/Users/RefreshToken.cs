using Common.Domain.Abstractions;

namespace Modules.Users.Domain.Users;
public sealed class RefreshToken : Entity<Guid>
{
    private RefreshToken(Guid id, string token, Guid userId, DateTime expiresOnUtc) : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    private RefreshToken()
    {
    }
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public User User { get; }

    public static RefreshToken Create(string token, Guid userId, DateTime expiresOnUtc)
    {
        var refreshToken = new RefreshToken(Guid.NewGuid(), token, userId, expiresOnUtc);

        return refreshToken;
    }

    public static RefreshToken Update(RefreshToken refreshToken, string token, DateTime expiresOnUtc)
    {
        refreshToken.Token = token;
        refreshToken.ExpiresOnUtc = expiresOnUtc;

        return refreshToken;
    }
}