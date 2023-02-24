namespace OneDriveIntegrator.Services.Subscription.Models;

public class SubscriptionOptions
{
    public static string OptionSectionName = "Subscription";
    
    public string NotificationUrl { get; set; } = default!;

    public int ExpirationInDays { get; set; }

    public string ClientState { get; set; } = default!;
}