using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Application.Users.Update;
using Modules.Users.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Application.Users.GetAll;
public sealed class GetAllUserQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAllUserQuery, IReadOnlyList<UserResponse>>
{
    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<User> users = await userRepository.GetAllAsync(cancellationToken);

        if (users is null || users.Count == 0)
        {
            return Result.Failure<IReadOnlyList<UserResponse>>(UserErrors.NotFound);
        }

        var userResponse = users.Select(user => new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedOnUtc,
            user.UpdatedOnUtc
        )).ToList();

        return userResponse;
    }
}
