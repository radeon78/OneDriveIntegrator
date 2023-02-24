using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Subscription.Models;

namespace OneDriveIntegrator.Services.Subscription;

public class SubscriptionService : ISubscriptionService
{
    private readonly TableClient _subscriptionTableClient;
    private readonly TableClient _notificationTableClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SubscriptionOptions _subscriptionOptions;

    private const string SubscriptionTableName = "Subscriptions";
    private const string NotificationTableName = "Notifications";

    public SubscriptionService(
        TableServiceClient tableServiceClient,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptionsSnapshot<SubscriptionOptions> subscriptionOptions)
    {
        _subscriptionTableClient = tableServiceClient.GetTableClient(tableName: SubscriptionTableName);
        _notificationTableClient = tableServiceClient.GetTableClient(tableName: NotificationTableName);
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _subscriptionOptions = subscriptionOptions.Get(OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task Subscribe(string itemId)
    {
        await _subscriptionTableClient.CreateIfNotExistsAsync();
        var user = _httpContextAccessor.GetSignedInUser();
        var subscriptionEntity = await GetSubscriptionFromStorage(user);

        if (subscriptionEntity.HasValue)
        {
            await UpdateSubscriptionInStorage(subscriptionEntity.Value.UpdateFolderToScan(itemId));
            return;
        }

        var subscriptionResponse = await RegisterSubscription();
        await AddSubscriptionToStorage(SubscriptionEntity.Create(
            id: subscriptionResponse.Id,
            itemId: itemId,
            applicationId: subscriptionResponse.ApplicationId,
            creatorId: subscriptionResponse.CreatorId,
            changeType: subscriptionResponse.ChangeType,
            notificationUrl: subscriptionResponse.NotificationUrl,
            resource: subscriptionResponse.Resource,
            expirationDateTime: subscriptionResponse.ExpirationDateTime,
            user: user));
    }

    public async Task Unsubscribe(string itemId)
    {
        var user = _httpContextAccessor.GetSignedInUser();
        var subscriptionEntity = await GetSubscriptionFromStorage(user);

        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.HttpGraphClientName)
            .DeleteAsync($"/v1.0/subscriptions/{subscriptionEntity.Value.Id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException(body);
        }

        await DeleteSubscriptionFromStorage(user);
    }

    public async Task<SubscriptionEntity?> GetSubscription(string itemId)
    {
        var user = _httpContextAccessor.GetSignedInUser();
        var subscriptionEntity = await GetSubscriptionFromStorage(user);

        if (!subscriptionEntity.HasValue)
            return null;

        return subscriptionEntity.Value.ItemId == itemId
            ? subscriptionEntity.Value
            : null;
    }

    public async Task AddNotification(Notification notification)
    {
        await _notificationTableClient.CreateIfNotExistsAsync();
        await AddNotificationToStorage(
            NotificationEntity.Create(notification: JsonConvert.SerializeObject(notification)));
    }
    
    private async Task<SubscriptionResponse> RegisterSubscription()
    {
        var request = new SubscriptionRequest(
            notificationUrl: _subscriptionOptions.NotificationUrl,
            resource: "/me/drive/root",
            expirationInDays: _subscriptionOptions.ExpirationInDays,
            clientState: _subscriptionOptions.ClientState);

        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.HttpGraphClientName)
            .PostAsJsonAsync("/v1.0/subscriptions", request);

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new InvalidOperationException(body);

        return JsonConvert.DeserializeObject<SubscriptionResponse>(body)
               ?? throw new InvalidOperationException(Constants.NullResponseMessage);
    }

    private Task<NullableResponse<SubscriptionEntity>> GetSubscriptionFromStorage(string user)
        => _subscriptionTableClient.GetEntityIfExistsAsync<SubscriptionEntity>(partitionKey: user, rowKey: user);

    private Task AddSubscriptionToStorage(SubscriptionEntity subscriptionEntity)
        => _subscriptionTableClient.AddEntityAsync(subscriptionEntity);

    private Task UpdateSubscriptionInStorage(SubscriptionEntity subscriptionEntity)
        => _subscriptionTableClient.UpdateEntityAsync(subscriptionEntity, ETag.All);

    private async Task DeleteSubscriptionFromStorage(string user)
        => await _subscriptionTableClient.DeleteEntityAsync(user, user, ETag.All);

    private Task AddNotificationToStorage(NotificationEntity notificationEntity)
        => _notificationTableClient.AddEntityAsync(notificationEntity);
}