using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.Graph.Models;

public class Details : Item
{
    [JsonProperty("@odata.context")] 
    public string Context { get; init; } = default!;

    [JsonProperty("@microsoft.graph.downloadUrl")]
    public string? DownloadUrl { get; init; }
}