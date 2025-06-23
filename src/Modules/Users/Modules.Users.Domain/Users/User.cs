using Common.Domain.Abstractions;

namespace Modules.Users.Domain.Users;
public sealed class User : Entity<Guid>
{
    private User(Guid id, string firstName, string lastName, string email, string passwordHash, DateTime createdOnUtc) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        CreatedOnUtc = createdOnUtc;
    }

    private User()
    {
    }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static User Create(string firstName, string lastName, string email, string passwordHash, DateTime createdOnUtc)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, createdOnUtc);

        return user;
    }

    public static User Update(User user, string firstName, string lastName, DateTime updatedOnUtc)
    {
        user.FirstName = firstName;
        user.LastName = lastName;
        user.UpdatedOnUtc = updatedOnUtc;

        return user;
    }
}
