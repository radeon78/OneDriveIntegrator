using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OneDriveIntegrator.Services.Subscription;
using OneDriveIntegrator.Services.Subscription.Models;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class SubscriptionCollectionExtensions
{
    public static IServiceCollection AddSubscription(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<SubscriptionOptions>(OpenIdConnectDefaults.AuthenticationScheme,
                configuration.GetSection(SubscriptionOptions.OptionSectionName))
            .AddScoped<ISubscriptionService, SubscriptionService>();
}