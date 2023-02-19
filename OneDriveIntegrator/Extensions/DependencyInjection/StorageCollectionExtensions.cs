using Azure.Identity;
using Microsoft.Extensions.Azure;
using OneDriveIntegrator.Services.Token;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class StorageCollectionExtensions
{
    private const string StorageAccountConnectionStringKey = "StorageAccount:ConnectionString";

    public static IServiceCollection AddTokensStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(
                connectionString: configuration.GetSection(StorageAccountConnectionStringKey).Value);

            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        return services.AddScoped<ITokenService, TokenService>();
    }
}