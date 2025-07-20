using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Application.Accounts.Create;
public sealed class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateAccountCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create(request.UserId, request.Name, request.Type, new Money(request.BalanceAmount, Currency.Create(request.BalanceCurrency)), dateTimeProvider.UtcNow);

        accountRepository.Add(account);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
