namespace Modules.Users.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
}
