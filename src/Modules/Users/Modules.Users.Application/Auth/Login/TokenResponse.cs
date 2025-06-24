namespace Modules.Users.Application.Auth.Login;
public sealed record TokenResponse(string AccessToken, string RefreshToken);