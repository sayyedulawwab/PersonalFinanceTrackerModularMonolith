using Modules.Users.Domain.Users;

namespace Modules.Users.Domain.UnitTests.Users;
public class UserTests
{
    [Fact]
    public void Create_ShouldInitializeUserWithCorrectValues()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string email = "john.doe@example.com";
        string passwordHash = "hashed";
        DateTime createdOn = DateTime.UtcNow;

        // Act
        var user = User.Create(firstName, lastName, email, passwordHash, createdOn);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(createdOn, user.CreatedOnUtc);
        Assert.Null(user.UpdatedOnUtc);
    }

    [Fact]
    public void Update_ShouldChangeUserNamesAndSetUpdatedOn()
    {
        // Arrange
        var user = User.Create("OldFirst", "OldLast", "old@example.com", "oldHash", DateTime.UtcNow);
        string newFirstName = "NewFirst";
        string newLastName = "NewLast";
        DateTime updatedOn = DateTime.UtcNow.AddHours(1);

        // Act
        var updatedUser = User.Update(user, newFirstName, newLastName, updatedOn);

        // Assert
        Assert.Equal(user.Id, updatedUser.Id); // still the same user
        Assert.Equal(newFirstName, updatedUser.FirstName);
        Assert.Equal(newLastName, updatedUser.LastName);
        Assert.Equal(updatedOn, updatedUser.UpdatedOnUtc);
    }
}
