namespace OneDriveIntegrator.Services.Subscription.Models;

public class SubscriptionResponse
{
    public string Id { get; set; } = default!;

    public string ApplicationId { get; init; } = default!;

    public string CreatorId { get; init; } = default!;

    public string ChangeType { get; init; } = default!;

    public string NotificationUrl { get; init; } = default!;

    public string Resource { get; init; } = default!;

    public DateTime ExpirationDateTime { get; init; }

    public string ClientState { get; init; } = default!;
}