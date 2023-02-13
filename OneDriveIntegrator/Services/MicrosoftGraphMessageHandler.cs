using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace OneDriveIntegrator.Services;

public class MicrosoftGraphMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public MicrosoftGraphMessageHandler(IHttpContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_contextAccessor.HttpContext == null) 
            throw new ArgumentNullException(nameof(_contextAccessor.HttpContext));
        
        var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentNullException(nameof(accessToken));
        
        request.Headers.Add(HeaderNames.Authorization, new[]{$"Bearer {accessToken}"});

        return await base.SendAsync(request, cancellationToken);
    }
}