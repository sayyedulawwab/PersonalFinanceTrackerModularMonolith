using Common.Application.Abstractions.Messaging;
using Modules.Accounts.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Accounts.Application.Accounts.Create;
public record CreateAccountCommand(Guid UserId, string Name, AccountType Type, decimal BalanceAmount, string BalanceCurrency) : ICommand<Guid>;
