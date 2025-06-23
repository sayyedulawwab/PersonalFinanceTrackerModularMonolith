using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.Register;

public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password) : ICommand<Guid>;