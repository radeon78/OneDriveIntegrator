using Newtonsoft.Json;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Graph.Models;

namespace OneDriveIntegrator.Services.Graph;

public class GraphClient : IGraphClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GraphClient(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public async Task<Items> GetRootChildren()
        => await Get<Items>("/v1.0/me/drive/root/children");

    public async Task<Items> GetItemChildren(string id)
        => await Get<Items>($"/v1.0/me/drive/items/{id}/children");

    public async Task<Details> GetItemDetails(string id)
        => await Get<Details>($"/v1.0/me/drive/items/{id}");

    private async Task<T> Get<T>(string url)
    {
        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.HttpGraphClientName)
            .GetAsync(url);

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new InvalidOperationException(body);

        return JsonConvert.DeserializeObject<T>(body)
               ?? throw new InvalidOperationException(Constants.NullResponseMessage);
    }
}