using System.Net;
using Microsoft.Extensions.Options;
using CSharpApp.Core.Settings;
using CSharpApp.Application.AuthService;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Tests;

public class UnitTestAuthService
{
    [Fact]
    public async Task Login_ReturnTokens_WheApiReturnsOk()
    {
        var request = new LoginRequest
        {
            Email = "john@mail.com",
            Password = "changeme"
        };

        var loginResponse = """
                            {
                            "access_token": "test-access-token",
                            "refresh_token": "test-refresh-token"
                            }
                            """;

        var response = CommonServices.CreateHttpResponse(loginResponse, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.Login(request);
        Assert.NotNull(result);
        Assert.Equal("test-access-token", result.AccessToken);
        Assert.Equal("test-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task Login_Throws_WhenApiReturnsError()
    {
        var request = new LoginRequest
        {
            Email = "john@mail.com",
            Password = "changeme"
        };

        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var service = CreateService(response);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.Login(request));
    }

    [Fact]
    public async Task GetUserProfile_ReturnsUserProfile_WhenApiReturnsSuccess()
    {
        string bearerToken = "test-access-token";

        var userProfile = """
                            {
                                "id": 1,
                                "email": "john@mail.com",
                                "password": "changeme",
                                "name": "Jhon",
                                "role": "customer",
                                "avatar": "https://api.lorem.space/image/face?w=640&h=480&r=867"
                            }
                        """;

        var response = CommonServices.CreateHttpResponse(userProfile, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.GetUserProfile(bearerToken);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("john@mail.com", result.Email);
    }

    [Fact]
    public async Task GetUserProfile_Throws_WhenApiReturnsUnauthorized()
    {
        string bearerToken = "invalid-token";

        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var service = CreateService(response);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserProfile(bearerToken));
    }

    [Fact]
    public async Task RefreshToken_ReturnsLoginResponse_WhenApiReturnsSuccess()
    {
        var request = new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        };

        var loginResponse = """
                            {
                                "access_token": "test-access-token",
                                "refresh_token": "test-refresh-token"
                            }
                            """;

        var response = CommonServices.CreateHttpResponse(loginResponse, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.RefreshToken(request);
        Assert.NotNull(result);
        Assert.Equal("test-access-token", result.AccessToken);
        Assert.Equal("test-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_Throws_WhenApiReturnsUnauthorized()
    {
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-refresh-token"
        };

        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var service = CreateService(response);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.RefreshToken(request));
    }
    
    private static AuthService CreateService(HttpResponseMessage response) // Helper method to create Service with mocked HttpClient
    {
        var handler = new CommonServices.HttpMessageHandlerStub(response);

        var options = Options.Create(new RestApiSettings
        {
            BaseUrl = "http://testapi/v1/",
            Auth = "auth/login",
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.Value.BaseUrl!)
        };

        return new AuthService(httpClient, options);
    }

}
