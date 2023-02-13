using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.MicrosoftGraph.Models;

public class Item
{
    [JsonProperty("createdDateTime")] 
    public DateTime Created { get; set; }

    [JsonProperty("lastModifiedDateTime")] 
    public DateTime LastModifiedDate { get; set; }

    [JsonProperty("id")] 
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;

    [JsonProperty("size")] 
    public long Size { get; set; }

    [JsonProperty("folder")] 
    public Folder? Folder { get; set; }

    [JsonProperty("file")] 
    public File? File { get; set; }

    public bool IsFile() => File != null;
}