using System.Net.Http.Headers;
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

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_restApiSettings.Auth}/login", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<UserProfileResponse?> GetUserProfile(string bearerToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_restApiSettings.Auth}/profile");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserProfileResponse>();
    }

    public async Task<LoginResponse?> RefreshToken(RefreshTokenRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_restApiSettings.Auth}/refresh-token", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
}