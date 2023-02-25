using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Graph.Models;

namespace OneDriveIntegrator.Services.Graph;

public class GraphClient : IGraphClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GraphClient(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public Task<Items> GetRootChildren()
        => Get<Items>("/v1.0/me/drive/root/children");

    public Task<Items> GetItemChildren(string id)
        => Get<Items>($"/v1.0/me/drive/items/{id}/children");

    public Task<Details> GetItemDetails(string id)
        => Get<Details>($"/v1.0/me/drive/items/{id}");

    public Task<Details> GetItemDetailsOffline(string id, string accessToken)
        => Get<Details>($"/v1.0/me/drive/items/{id}", accessToken);

    private Task<T> Get<T>(string url, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add(HeaderNames.Authorization, new[] { $"Bearer {accessToken}" });
        return Send<T>(request);
    }

    private Task<T> Get<T>(string url)
        => Send<T>(new HttpRequestMessage(HttpMethod.Get, url));

    private async Task<T> Send<T>(HttpRequestMessage request)
    {
        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.HttpGraphClientName)
            .SendAsync(request);

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new InvalidOperationException(body);

        return JsonConvert.DeserializeObject<T>(body)
               ?? throw new InvalidOperationException(Constants.NullResponseMessage);
    }
}