using Azure;
using Azure.Data.Tables;

namespace OneDriveIntegrator.Services.Subscription.Models;

public class NotificationEntity : ITableEntity
{
    public string Notification { get; set; } = default!;

    public string PartitionKey { get; set; } = default!;

    public string RowKey { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public static NotificationEntity Create(string notification)
    {
        var guid = Guid.NewGuid();
        return new NotificationEntity()
        {
            Notification = notification,
            RowKey = guid.ToString(),
            PartitionKey = guid.ToString()
        };
    }
}