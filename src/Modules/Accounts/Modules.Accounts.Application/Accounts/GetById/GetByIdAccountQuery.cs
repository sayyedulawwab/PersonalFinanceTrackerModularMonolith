using Common.Application.Abstractions.Messaging;

namespace Modules.Accounts.Application.Accounts.GetById;
public record GetByIdAccountQuery(Guid AccountId) : IQuery<AccountResponse>;
