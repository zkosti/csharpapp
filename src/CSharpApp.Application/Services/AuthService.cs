using System.Net.Http.Json;

namespace CSharpApp.Application.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;

    public AuthService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
    }

    public async Task<LoginResponse?> Login()
    {
        var request = new LoginRequest
        {
            Email = _restApiSettings.Username,
            Password = _restApiSettings.Password
        };

        var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Auth, request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
}