using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.MicrosoftGraph.Models;

public class ItemsResponse
{
    [JsonProperty("@odata.context")] 
    public string Context { get; set; } = string.Empty;

    [JsonProperty("@odata.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<Item> Items { get; set; } = new(0);
}