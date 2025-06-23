using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Configurations;
internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token).HasMaxLength(200);

        builder.HasIndex(r => r.Token).IsUnique();

        builder.Property(r => r.ExpiresOnUtc);

        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId);
    }
}
