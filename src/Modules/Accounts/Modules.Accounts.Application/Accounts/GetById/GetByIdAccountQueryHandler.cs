using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Application.Accounts.GetById;
public sealed class GetByIdAccountQueryHandler(IAccountRepository accountRepository)
    : IQueryHandler<GetByIdAccountQuery, AccountResponse>
{
    public async Task<Result<AccountResponse>> Handle(GetByIdAccountQuery request, CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<AccountResponse>(AccountErrors.NotFound);
        }

        var userResponse = new AccountResponse(
            account.Id,
            account.Name,
            account.Type.ToString(),
            account.Balance.Amount,
            account.Balance.Currency.Code,
            account.CreatedOnUtc,
            account.UpdatedOnUtc
        );

        return userResponse;
    }
}
