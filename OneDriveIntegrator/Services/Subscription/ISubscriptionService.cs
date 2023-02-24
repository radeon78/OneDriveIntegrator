using OneDriveIntegrator.Services.Subscription.Models;

namespace OneDriveIntegrator.Services.Subscription;

public interface ISubscriptionService
{
    Task Subscribe(string itemId);

    Task Unsubscribe(string itemId);
    
    Task<SubscriptionEntity?> GetSubscription(string itemId);

    Task AddNotification(Notification notification);
}