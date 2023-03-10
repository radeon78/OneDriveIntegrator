using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class HttpClientCollectionExtensions
{
    private const string OneDriveApiUrlKey = "OneDriveApi:Url";

    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<MicrosoftGraphMessageHandler>()
            .AddHttpClient(Constants.HttpGraphClientName,
                client => { client.BaseAddress = new Uri(configuration.GetSection(OneDriveApiUrlKey).Value); })
            .AddHttpMessageHandler<MicrosoftGraphMessageHandler>();

        services.AddHttpClient(Constants.HttpAuthenticationClientName,
            client =>
            {
                client.BaseAddress = new Uri(AuthenticationCollectionExtensions
                    .GetOpenIdConnectConfigurationValue(configuration, nameof(OpenIdConnectOptions.Authority))
                    .Replace("/common/v2.0", string.Empty));
            });

        return services;
    }
}