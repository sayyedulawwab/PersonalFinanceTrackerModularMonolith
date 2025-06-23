namespace Modules.Users.Domain.Users;
public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);
    void Update(RefreshToken token);
    void Remove(RefreshToken token);
    Task DeleteByUserIdAsync(Guid userId);
    Task<RefreshToken?> GetByTokenAsync(string token);
}
