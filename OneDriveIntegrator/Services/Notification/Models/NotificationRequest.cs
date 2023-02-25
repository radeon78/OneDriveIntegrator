namespace OneDriveIntegrator.Services.Notification.Models;

public class NotificationRequest
{
    public List<Value> Value { get; init; } = new();
}

public class Value
{
    public string SubscriptionId { get; set; } = default!;

    public string Resource { get; init; } = default!;

    public string ChangeType { get; init; } = default!;

    public string ClientState { get; init; } = default!;
}