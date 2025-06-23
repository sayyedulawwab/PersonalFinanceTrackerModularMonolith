using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
    string GenerateRefreshToken();
}
