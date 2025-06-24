using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Application.Users.Update;
using Modules.Users.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Application.Users.GetById;
public sealed class GetByIdUserQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetByIdUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }

        var userResponse = new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedOnUtc,
            user.UpdatedOnUtc
        );

        return userResponse;
    }
}
