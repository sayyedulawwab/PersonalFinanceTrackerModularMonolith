using Common.Application.Abstractions.Messaging;

namespace Modules.Users.Application.Users.GetById;
public record GetByIdUserQuery(Guid UserId) : IQuery<UserResponse>;
