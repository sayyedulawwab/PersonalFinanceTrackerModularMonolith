using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Application.Accounts.Delete;
public sealed class DeleteAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteAccountCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<Guid>(AccountErrors.NotFound);
        }

        accountRepository.Remove(account);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
