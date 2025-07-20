using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Application.Accounts.GetAll;
public sealed class GetAllAccountsQueryHandler(IAccountRepository accountRepository)
    : IQueryHandler<GetAllAccountsQuery, IReadOnlyList<AccountResponse>>
{
    public async Task<Result<IReadOnlyList<AccountResponse>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Account> accounts = await accountRepository.GetAllAsync(cancellationToken);

        var accountResponse = accounts.Select(account => new AccountResponse(
            account.Id,
            account.Name,
            account.Type.ToString(),
            account.Balance.Amount,
            account.Balance.Currency.Code,
            account.CreatedOnUtc,
            account.UpdatedOnUtc
        )).ToList();

        return accountResponse;
    }
}
