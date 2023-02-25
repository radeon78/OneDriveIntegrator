namespace OneDriveIntegrator.Services.Token.Models;

public class TokenInput
{
    public TokenInput(string accessToken, string refreshToken, string idToken, long expiresIn)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        ExpiresIn = expiresIn;
    }

    public string AccessToken { get; }

    public string RefreshToken { get; }

    public string IdToken { get; }

    public long ExpiresIn { get; }
}