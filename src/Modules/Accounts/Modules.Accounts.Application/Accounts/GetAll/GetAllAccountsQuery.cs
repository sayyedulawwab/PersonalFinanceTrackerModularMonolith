using Common.Application.Abstractions.Messaging;
using Modules.Accounts.Application.Accounts;

namespace Modules.Accounts.Application.Accounts.GetAll;
public record GetAllAccountsQuery() : IQuery<IReadOnlyList<AccountResponse>>;
