using Common.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Accounts.Domain.Accounts;
public sealed class Account : Entity<Guid>
{
    private Account(Guid id, Guid userId, string name, AccountType type, Money balance, DateTime createdOnUtc) : base(id)
    {
        UserId = userId;
        Name = name;
        Type = type;
        Balance = balance;
        CreatedOnUtc = createdOnUtc;
    }

    private Account()
    {
    }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public AccountType Type { get; private set; }
    public Money Balance { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Account Create(Guid userId, string name, AccountType type, Money balance, DateTime createdOnUtc)
    {
        var account = new Account(Guid.NewGuid(), userId, name, type, balance, createdOnUtc);

        return account;
    }

    public static Account Update(Account account, string name, AccountType type, Money balance, DateTime updatedOnUtc)
    {
        account.Name = name;
        account.Type = type;
        account.Balance = balance;
        account.UpdatedOnUtc = updatedOnUtc;

        return account;
    }
}
