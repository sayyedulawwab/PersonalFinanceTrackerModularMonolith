using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Application.Users;
public record UserResponse(Guid Id, string FirstName, string LastName, string Email, DateTime CreatedOnUtc, DateTime? UpdatedOnUtc);
