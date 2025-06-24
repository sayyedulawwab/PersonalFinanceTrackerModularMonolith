using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Auth.Login;
public sealed class LoginUserQueryHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider)
    : IQueryHandler<LoginUserQuery, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(
        LoginUserQuery request,
        CancellationToken cancellationToken)
    {

        User? user = await userRepository.GetByEmail(request.Email);

        if (user is null)
        {
            return Result.Failure<TokenResponse>(UserErrors.NotFound);
        }

        bool isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);
        }

        string accessToken = tokenProvider.Create(user);

        var refreshToken = RefreshToken.Create(tokenProvider.GenerateRefreshToken(), user.Id, DateTime.UtcNow.AddDays(7));

        refreshTokenRepository.Add(refreshToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken, refreshToken.Token);
    }

}