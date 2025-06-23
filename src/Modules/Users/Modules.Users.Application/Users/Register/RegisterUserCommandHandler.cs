using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Users.Register;
public sealed class RegisterUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User? existingUserByEmail = await userRepository.GetByEmail(request.Email);

        if (existingUserByEmail is not null)
        {
            return Result.Failure<Guid>(UserErrors.AlreadyExists);
        }

        string hashedPassword = passwordHasher.Hash(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword,
            dateTimeProvider.UtcNow);

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}