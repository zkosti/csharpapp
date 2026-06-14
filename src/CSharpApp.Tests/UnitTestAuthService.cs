using System.Net;
using Microsoft.Extensions.Options;
using CSharpApp.Core.Settings;

using CSharpApp.Application.AuthService;

namespace CSharpApp.Tests;

public class UnitTestAuthService
{
    [Fact]
    public async Task Login_ReturnTokens_WheApiReturnsOk()
    {
        var loginResponse = """
                            {
                            "access_token": "test-access-token",
                            "refresh_token": "test-refresh-token"
                            }
                            """;

        var response = CommonServices.CreateHttpResponse(loginResponse, HttpStatusCode.OK);

        var service = CreateService(response);

        var result = await service.Login();

        Assert.NotNull(result);
        Assert.Equal("test-access-token", result.AccessToken);
        Assert.Equal("test-refresh-token", result.RefreshToken);
    }
    
    [Fact]
    public async Task Login_Throws_WhenApiReturnsError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        var service = CreateService(response);

        await Assert.ThrowsAsync<HttpRequestException>(service.Login);
    }
    
    private static AuthService CreateService(HttpResponseMessage response) // Helper method to create Service with mocked HttpClient
    {
        var handler = new CommonServices.HttpMessageHandlerStub(response);

        var options = Options.Create(new RestApiSettings
        {
            BaseUrl = "http://testapi/v1/",
            Products = "auth/login",
            Username = "john@mail.com",
            Password = "changeme"
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.Value.BaseUrl!)
        };

        return new AuthService(httpClient, options);
    }

}
