namespace CSharpApp.Application.TokenService;

public class TokenService : ITokenService
{
    private readonly IAuthService _authService;
    private string? _accessToken;

    public TokenService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<string> GetAccessToken()
    {
        if (!string.IsNullOrWhiteSpace(_accessToken))
        {
            return _accessToken;
        }

        var loginResponse = await _authService.Login();

        if (string.IsNullOrWhiteSpace(loginResponse?.AccessToken))
        {
            throw new InvalidOperationException("Unable to retrieve access token.");
        }

        _accessToken = loginResponse.AccessToken;

        return _accessToken;
    }
}