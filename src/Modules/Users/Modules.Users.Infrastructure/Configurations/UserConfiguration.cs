using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
               .HasMaxLength(200);

        builder.Property(user => user.LastName)
               .HasMaxLength(200);


        builder.Property(user => user.Email)
               .HasMaxLength(200);

        builder.Property(user => user.PasswordHash);

        builder.Property(user => user.CreatedOnUtc);

        builder.Property(user => user.UpdatedOnUtc);
    }
}
