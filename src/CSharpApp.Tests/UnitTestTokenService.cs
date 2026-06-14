using CSharpApp.Core.Dtos;

using CSharpApp.Application.TokenService;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Tests;

public class UnitTestTokenService
{
    [Fact]
    public async Task GetAccessToken_ReturnsToken()
    {
        var fakeAuthService = new FakeAuthService(new LoginResponse
        {
            AccessToken = "test-token",
            RefreshToken = "test-refresh"
        });

        var tokenService = new TokenService(fakeAuthService);

        var token = await tokenService.GetAccessToken();

        Assert.Equal("test-token", token);
    }

    [Fact]
    public async Task GetAccessToken_CallsLoginOnlyOnce_WhenCalledTwice()
    {
        var fakeAuthService = new FakeAuthService(new LoginResponse
        {
            AccessToken = "test-token"
        });

        var tokenService = new TokenService(fakeAuthService);

        await tokenService.GetAccessToken();
        await tokenService.GetAccessToken();

        Assert.Equal(1, fakeAuthService.CallCount);
    }

    [Fact]
    public async Task GetAccessToken_Throws_WhenAccessTokenIsMissing()
    {
        var fakeAuthService = new FakeAuthService(new LoginResponse
        {
            AccessToken = null
        });

        var tokenService = new TokenService(fakeAuthService);

        await Assert.ThrowsAsync<InvalidOperationException>(tokenService.GetAccessToken);
    }

    private class FakeAuthService : IAuthService
    {
        private readonly LoginResponse? _response;
        public int CallCount { get; private set; }

        public FakeAuthService(LoginResponse? response)
        {
            _response = response;
        }

        public Task<LoginResponse?> Login()
        {
            CallCount++;
            return Task.FromResult(_response);
        }
    }

}
