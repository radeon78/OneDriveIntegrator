namespace OneDriveIntegrator.Services.Subscription.Models;

public class SubscriptionRequest
{
    public SubscriptionRequest(string notificationUrl, string resource, int expirationInDays, string clientState)
    {
        NotificationUrl = notificationUrl;
        Resource = resource;
        ExpirationDateTime = DateTime.UtcNow.AddDays(expirationInDays);
        ClientState = clientState;
    }

    public string ChangeType { get; } = "updated";

    public string NotificationUrl { get; }

    public string Resource { get; }

    public DateTime ExpirationDateTime { get; }

    public string ClientState { get; }
}