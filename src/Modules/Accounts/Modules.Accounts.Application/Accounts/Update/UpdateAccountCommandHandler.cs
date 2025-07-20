using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Accounts.Application.Accounts.Create;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Application.Accounts.Update;
public sealed class UpdateAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateAccountCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<Guid>(AccountErrors.NotFound);
        }

        var updatedAccount = Account.Update(account, request.Name, request.Type, new Money(request.BalanceAmount, Currency.Create(request.BalanceCurrency)), dateTimeProvider.UtcNow);

        accountRepository.Update(updatedAccount);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return updatedAccount.Id;
    }
}
