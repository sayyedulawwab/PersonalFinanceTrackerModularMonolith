using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Accounts.Domain.Accounts;

namespace Modules.Accounts.Infrastructure.Configurations;
internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Name)
               .HasMaxLength(200);

        builder.Property(account => account.Type)
               .HasMaxLength(200);

        builder.OwnsOne(account => account.Balance, priceBuilder =>
        {
            priceBuilder.Property(money => money.Amount)
                        .HasColumnType("decimal(18, 2)");

            priceBuilder.Property(money => money.Currency)
                        .HasConversion(currency => currency.Code, code => Currency.FromCode(code))
                        .HasDefaultValue(Currency.FromCode("BDT"));

        });

        builder.Property(user => user.CreatedOnUtc);
        builder.Property(user => user.UpdatedOnUtc);
    }
}
