using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.GetAll;
public record GetAllUserQuery() : IQuery<IReadOnlyList<UserResponse>>;
