using OneDriveIntegrator.Services.Notification;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class NotificationCollectionExtensions
{
    public static IServiceCollection AddINotification(this IServiceCollection services)
        => services.AddScoped<INotificationService, NotificationService>();
}