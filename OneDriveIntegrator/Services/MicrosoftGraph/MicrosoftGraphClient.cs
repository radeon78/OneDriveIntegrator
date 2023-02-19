using Newtonsoft.Json;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.MicrosoftGraph.Models;

namespace OneDriveIntegrator.Services.MicrosoftGraph;

public class MicrosoftGraphClient : IMicrosoftGraphClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MicrosoftGraphClient(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public async Task<Items> GetRootChildren()
        => await Get<Items>("/v1.0/me/drive/root/children");

    public async Task<Details> GetItemDetails(string id)
        => await Get<Details>($"/v1.0/me/drive/items/{id}");

    public async Task<Items> GetItemChildren(string id)
        => await Get<Items>($"/v1.0/me/drive/items/{id}/children");

    private async Task<T> Get<T>(string url)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.GraphClientName)
            .SendAsync(httpRequest);

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(body) ??
                   throw new InvalidOperationException("Response is null or empty");

        throw new InvalidOperationException(body);
    }
}