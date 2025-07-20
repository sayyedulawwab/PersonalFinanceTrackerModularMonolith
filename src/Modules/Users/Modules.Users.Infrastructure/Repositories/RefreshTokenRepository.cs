using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Repositories;
internal sealed class RefreshTokenRepository(UsersDbContext dbContext) : Repository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        RefreshToken? refreshToken = await DbContext.Set<RefreshToken>()
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);

        return refreshToken;
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        await DbContext.Set<RefreshToken>()
            .Where(r => r.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
