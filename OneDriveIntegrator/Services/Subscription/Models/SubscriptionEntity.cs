using Azure;
using Azure.Data.Tables;

namespace OneDriveIntegrator.Services.Subscription.Models;

public class SubscriptionEntity : ITableEntity
{
    public string Id { get; set; } = default!;

    public string ItemId { get; set; } = default!;

    public string ApplicationId { get; set; } = default!;

    public string CreatorId { get; set; } = default!;

    public string ChangeType { get; init; } = default!;

    public string NotificationUrl { get; init; } = default!;

    public string Resource { get; init; } = default!;

    public DateTime ExpirationDateTime { get; init; }

    public string PartitionKey { get; set; } = default!;

    public string RowKey { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; } = default!;

    public ETag ETag { get; set; } = default!;

    public static SubscriptionEntity Create(
        string id,
        string itemId,
        string applicationId,
        string creatorId,
        string changeType,
        string notificationUrl,
        string resource,
        DateTime expirationDateTime,
        string user)
        => new()
        {
            Id = id,
            ItemId = itemId,
            ApplicationId = applicationId,
            CreatorId = creatorId,
            ChangeType = changeType,
            NotificationUrl = notificationUrl,
            Resource = resource,
            ExpirationDateTime = expirationDateTime,
            RowKey = user,
            PartitionKey = user
        };

    public SubscriptionEntity UpdateFolderToScan(string itemId)
    {
        ItemId = itemId;
        return this;
    }
}