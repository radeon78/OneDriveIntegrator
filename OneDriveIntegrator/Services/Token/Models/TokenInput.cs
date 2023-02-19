namespace OneDriveIntegrator.Services.Token.Models;

public class TokenInput
{
    public TokenInput(string accessToken, string refreshToken, string idToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
    }
    public string AccessToken { get; }
    
    public string RefreshToken { get; }
    
    public string IdToken { get; }
}