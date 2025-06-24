using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Modules.Users.Application.Abstractions.Authentication;
using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Auth.Login;
public sealed class LoginUserWithRefreshTokenQueryHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenProvider)
    : IQueryHandler<LoginUserWithRefreshTokenQuery, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(LoginUserWithRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            return Result.Failure<TokenResponse>(RefreshTokenErrors.Expired);
        }

        string accessToken = tokenProvider.Create(refreshToken.User);

        refreshToken = RefreshToken.Update(refreshToken, tokenProvider.GenerateRefreshToken(), DateTime.UtcNow.AddDays(7));

        refreshTokenRepository.Update(refreshToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new TokenResponse(accessToken, refreshToken.Token);

        return response;
    }
}