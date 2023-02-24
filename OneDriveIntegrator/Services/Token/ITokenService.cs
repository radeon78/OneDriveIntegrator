using OneDriveIntegrator.Services.Token.Models;

namespace OneDriveIntegrator.Services.Token;

public interface ITokenService
{
    Task AddOrUpdateToken(TokenInput tokenInput);

    Task<TokenEntity> GetTokenAndRefreshIfNeed();
}