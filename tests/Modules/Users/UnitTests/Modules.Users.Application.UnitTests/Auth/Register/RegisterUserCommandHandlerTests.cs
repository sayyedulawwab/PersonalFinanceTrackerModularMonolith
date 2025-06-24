using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Application.Auth.Register;
using Modules.Users.Domain.Users;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Auth.Register;
public class RegisterUserCommandHandlerTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        var existingUser = User.Create("John", "Doe", "john@example.com", "hashed", DateTime.UtcNow);

        _userRepository.GetByEmail("john@example.com").Returns(existingUser); // simulate existing user

        var handler = new RegisterUserCommandHandler(_passwordHasher, _userRepository, _unitOfWork, _dateTimeProvider);

        var command = new RegisterUserCommand("John", "Doe", "john@example.com", "password");

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyExists, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidInput()
    {
        _userRepository.GetByEmail("john@example.com").Returns((User?)null);
        _passwordHasher.Hash("password").Returns("hashed");
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var handler = new RegisterUserCommandHandler(_passwordHasher, _userRepository, _unitOfWork, _dateTimeProvider);

        var command = new RegisterUserCommand("John", "Doe", "john@example.com", "password");

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        _userRepository.Received(1).Add(Arg.Any<User>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
