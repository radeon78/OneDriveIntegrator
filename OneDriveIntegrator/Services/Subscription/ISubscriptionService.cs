using OneDriveIntegrator.Services.Subscription.Models;

namespace OneDriveIntegrator.Services.Subscription;

public interface ISubscriptionService
{
    Task Subscribe(string itemId);

    Task Unsubscribe(string itemId);

    Task Update(SubscriptionEntity subscriptionEntity);

    Task<SubscriptionEntity?> GetSubscription(string itemId);

    Task<IEnumerable<SubscriptionEntity>> GetSubscriptions(IEnumerable<string> itemIds);
}