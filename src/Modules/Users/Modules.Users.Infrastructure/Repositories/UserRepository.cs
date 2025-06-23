using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Repositories;
internal sealed class UserRepository(ApplicationDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmail(string email)
    {
        User? user = await DbContext.Set<User>()
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();

        return user;

    }

}
