using Common.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Application.Users.Update;
public record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand<Guid>;
