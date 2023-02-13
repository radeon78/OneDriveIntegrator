using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.MicrosoftGraph.Models;

public class Folder
{
    [JsonProperty("childCount")]
    public int ChildCount { get; set; }
}