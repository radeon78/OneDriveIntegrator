using Newtonsoft.Json;
using OneDriveIntegrator.Services.MicrosoftGraph.Models;

namespace OneDriveIntegrator.Services.MicrosoftGraph;

public class MicrosoftGraphClient : IMicrosoftGraphClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public MicrosoftGraphClient(HttpClient httpClient, Uri baseUrl)
        => (_httpClient, _baseUrl) = (httpClient, baseUrl);

    public async Task<ItemsResponse> GetRootChildren()
        => await Get<ItemsResponse>(
            new Uri(_baseUrl, "/v1.0/me/drive/root/children"));

    public async Task<DetailsResponse> GetItemDetails(string id)
        => await Get<DetailsResponse>(
            new Uri(_baseUrl, $"/v1.0/me/drive/items/{id}"));

    public async Task<ItemsResponse> GetItemChildren(string id)
        => await Get<ItemsResponse>(
            new Uri(_baseUrl, $"/v1.0/me/drive/items/{id}/children"));
    
    private async Task<T> Get<T>(Uri url)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        var httpResponse = await _httpClient.SendAsync(httpRequest);
        var body = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(body) ??
                   throw new InvalidOperationException("Response is null or empty");

        throw new InvalidOperationException(body);
    }
}