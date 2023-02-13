using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.MicrosoftGraph.Models;

public class DetailsResponse : Item
{
    [JsonProperty("@odata.context")] 
    public string Context { get; set; } = string.Empty;

    [JsonProperty("@microsoft.graph.downloadUrl")]
    public string? DownloadUrl { get; set; }
}