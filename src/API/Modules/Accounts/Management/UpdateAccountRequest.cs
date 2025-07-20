using Modules.Accounts.Domain.Accounts;
using System.Text.Json.Serialization;

namespace API.Modules.Accounts.Management;

public record UpdateAccountRequest
{
    public string Name { get; init; }
    [JsonRequired] public AccountType Type { get; init; }
    [JsonRequired] public decimal BalanceAmount { get; init; }
    public string BalanceCurrency { get; init; }
}