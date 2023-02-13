using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.MicrosoftGraph.Models;

public class File
{
    [JsonProperty("mimeType")]
    public string Type { get; set; } = string.Empty;
}