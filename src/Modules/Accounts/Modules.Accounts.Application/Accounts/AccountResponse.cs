using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Accounts.Application.Accounts;
public record AccountResponse(Guid Id, string Name, string Type, decimal BalanceAmount, string BalanceCurrency, DateTime CreatedOnUtc, DateTime? UpdatedOnUtc);
