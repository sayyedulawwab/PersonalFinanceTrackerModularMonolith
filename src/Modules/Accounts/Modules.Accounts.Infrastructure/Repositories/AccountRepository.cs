using Microsoft.EntityFrameworkCore;
using Modules.Accounts.Domain.Accounts;
using Modules.Accounts.Infrastructure;

namespace Modules.Accounts.Infrastructure.Repositories;
internal sealed class AccountRepository(AccountsDbContext dbContext)
    : Repository<Account>(dbContext), IAccountRepository
{
    public async Task<IReadOnlyList<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Account> accounts = await DbContext
            .Set<Account>()
            .Where(account => account.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
