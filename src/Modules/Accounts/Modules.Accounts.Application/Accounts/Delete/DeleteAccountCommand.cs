using Common.Application.Abstractions.Messaging;
using Modules.Accounts.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Accounts.Application.Accounts.Delete;
public record DeleteAccountCommand(Guid AccountId) : ICommand<Guid>;
