using Common.Domain.Abstractions;

namespace Modules.Accounts.Domain.Accounts;
public static class AccountErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Account.NotFound",
        "The account with the specified identifier was not found");

    public static readonly Error ZeroBalance = Error.BadRequest(
        "Account.ZeroBalance",
        "The account has zero balance");
}