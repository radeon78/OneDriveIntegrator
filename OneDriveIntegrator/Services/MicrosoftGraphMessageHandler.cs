using Microsoft.Net.Http.Headers;
using OneDriveIntegrator.Services.Token;

namespace OneDriveIntegrator.Services;

public class MicrosoftGraphMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public MicrosoftGraphMessageHandler(ITokenService tokenService)
        => _tokenService = tokenService;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessToken();
        request.Headers.Add(HeaderNames.Authorization, new[] { $"Bearer {accessToken}" });
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetAccessToken()
    {
        var token = await _tokenService.GetTokenAndRefreshIfNeed();
        return token.AccessToken;
    }
}