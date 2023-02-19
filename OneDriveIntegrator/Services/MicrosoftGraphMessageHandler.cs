using Microsoft.Net.Http.Headers;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Token;

namespace OneDriveIntegrator.Services;

public class MicrosoftGraphMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ITokenService _tokenService;

    public MicrosoftGraphMessageHandler(IHttpContextAccessor contextAccessor, ITokenService tokenService)
        => (_contextAccessor, _tokenService) = (contextAccessor, tokenService);

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
        var user = GetSignedInUser();
        var token = await _tokenService.GetTokenAndRefreshIfNeed(user);
        return token.AccessToken;
    }

    private string GetSignedInUser()
        => _contextAccessor.HttpContext?.User.Claims.First(c => c.Type == Constants.UserClaimName).Value!;
}