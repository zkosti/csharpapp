using System.Net.Http.Headers;

namespace CSharpApp.Infrastructure.Configuration;

public class JwtHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public JwtHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessToken();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}