using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.Graph.Models;

public class Items
{
    [JsonProperty("@odata.context")] 
    public string Context { get; init; } = default!;

    [JsonProperty("@odata.count")] 
    public int Count { get; init; }

    public List<Item> Value { get; init; } = default!;
}