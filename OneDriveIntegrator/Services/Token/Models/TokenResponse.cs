using Newtonsoft.Json;

namespace OneDriveIntegrator.Services.Token.Models;

public class TokenResponse
{
    [JsonProperty("access_token")] 
    public string AccessToken { get; init; } = default!;

    [JsonProperty("refresh_token")] 
    public string RefreshToken { get; init; } = default!;

    [JsonProperty("id_token")] 
    public string IdToken { get; init; } = default!;
}