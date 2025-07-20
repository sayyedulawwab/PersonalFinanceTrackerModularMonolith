namespace Modules.Accounts.Domain.Accounts;
public interface IAccountRepository
{
    void Add(Account account);
    void Update(Account account);
    void Remove(Account account);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
