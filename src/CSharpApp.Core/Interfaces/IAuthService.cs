namespace CSharpApp.Core.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> Login(LoginRequest request);
    Task<UserProfileResponse?> GetUserProfile(string bearerToken);
    Task<LoginResponse?> RefreshToken(RefreshTokenRequest request);
    }