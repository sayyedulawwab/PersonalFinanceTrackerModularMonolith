using Common.Domain.Abstractions;

namespace Modules.Users.Domain.Users;
public static class RefreshTokenErrors
{
    public static readonly Error Expired = Error.BadRequest(
        "RefreshToken.Expired",
        "THe refresh token is expired");
}