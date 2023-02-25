using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDriveIntegrator.Services.Email;
using OneDriveIntegrator.Services.Email.Models;
using OneDriveIntegrator.Services.Graph;
using OneDriveIntegrator.Services.Graph.Models;
using OneDriveIntegrator.Services.Notification.Models;
using OneDriveIntegrator.Services.Subscription;
using OneDriveIntegrator.Services.Subscription.Models;
using OneDriveIntegrator.Services.Token;

namespace OneDriveIntegrator.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ITokenService _tokenService;
    private readonly IGraphClient _graphClient;
    private readonly IEmailService _emailService;
    private readonly TableClient _notificationTableClient;
    private readonly SubscriptionOptions _subscriptionOptions;

    private const string NotificationTableName = "Notifications";

    public NotificationService(
        TableServiceClient tableServiceClient,
        ISubscriptionService subscriptionService,
        ITokenService tokenService,
        IGraphClient graphClient,
        IEmailService emailService,
        IOptionsSnapshot<SubscriptionOptions> subscriptionOptions)
    {
        _subscriptionService = subscriptionService;
        _tokenService = tokenService;
        _graphClient = graphClient;
        _emailService = emailService;
        _notificationTableClient = tableServiceClient.GetTableClient(tableName: NotificationTableName);
        _subscriptionOptions = subscriptionOptions.Get(OpenIdConnectDefaults.AuthenticationScheme);
    }

    public bool Valid(NotificationRequest? request)
        => request?.Value != null &&
           request.Value.All(value => value.ClientState == _subscriptionOptions.ClientState);


    public async Task NotifyUserIfContentInSubscribedFolderChanged(NotificationRequest notificationRequest)
    {
        await _notificationTableClient.CreateIfNotExistsAsync();
        await AddNotificationToStorage(
            NotificationEntity.Create(notification: JsonConvert.SerializeObject(notificationRequest)));

        var subscriptions =
            await _subscriptionService.GetSubscriptions(notificationRequest.Value.Select(x => x.SubscriptionId));

        foreach (var subscription in subscriptions)
        {
            var token = await _tokenService.GetTokenAndRefreshIfNeed(user: subscription.RowKey);
            var details = await _graphClient.GetItemDetailsOffline(subscription.ItemId, token.AccessToken);

            if (!ChangedSomethingInSubscribedFolder(subscription, details)) continue;

            await _subscriptionService.Update(subscription.UpdateFolder(details.Folder!.ChildCount));
            await _emailService.Send(new EmailInput(
                emailTo: subscription.RowKey,
                textContent: CreateContent(subscription, details),
                folderName: details.Name));
        }
    }

    private Task AddNotificationToStorage(NotificationEntity notificationEntity)
        => _notificationTableClient.AddEntityAsync(notificationEntity);

    private static bool ChangedSomethingInSubscribedFolder(SubscriptionEntity subscription, Item item)
        => subscription.ItemChildCount != item.Folder!.ChildCount;

    private static string CreateContent(SubscriptionEntity subscription, Item item)
        => subscription.ItemChildCount > item.Folder!.ChildCount
            ? $"You deleted {subscription.ItemChildCount - item.Folder!.ChildCount} elements from '{item.Name}' folder."
            : $"You added {item.Folder!.ChildCount - subscription.ItemChildCount} elements to '{item.Name}' folder.";
}