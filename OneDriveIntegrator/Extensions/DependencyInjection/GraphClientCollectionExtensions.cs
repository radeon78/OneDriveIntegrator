using OneDriveIntegrator.Services.Graph;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class GraphClientCollectionExtensions
{
    public static IServiceCollection AddGraphClient(this IServiceCollection services)
        => services.AddScoped<IGraphClient, GraphClient>();
}