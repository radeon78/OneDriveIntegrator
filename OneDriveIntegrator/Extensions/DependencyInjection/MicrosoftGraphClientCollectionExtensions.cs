using OneDriveIntegrator.Services;
using OneDriveIntegrator.Services.MicrosoftGraph;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class MicrosoftGraphClientCollectionExtensions
{
    private const string ClientName = "MicrosoftGraphClient";
    private const string OneDriveApiUrlKey = "OneDriveApi:Url";

    public static IServiceCollection AddMicrosoftGraphClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<MicrosoftGraphMessageHandler>();

        services
            .AddHttpClient(ClientName)
            .AddHttpMessageHandler<MicrosoftGraphMessageHandler>();

        services.AddSingleton<IMicrosoftGraphClient, MicrosoftGraphClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(ClientName);
            var url = new Uri(configuration.GetSection(OneDriveApiUrlKey).Value);

            return new MicrosoftGraphClient(
                httpClient: httpClient,
                baseUrl: url);
        });

        return services;
    }
}