using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Token.Models;

namespace OneDriveIntegrator.Services.Token;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TableClient _tokenTableClient;
    private readonly OpenIdConnectOptions _openIdConnectOptions;

    private static readonly JwtSecurityTokenHandler Handler = new();

    private const string TokenTableName = "Tokens";
    private const string GrantType = "refresh_token";

    public TokenService(
        TableServiceClient tableServiceClient,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptionsSnapshot<OpenIdConnectOptions> openIdConnectOptions)
    {
        _tokenTableClient = tableServiceClient.GetTableClient(tableName: TokenTableName);
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _openIdConnectOptions = openIdConnectOptions.Get(OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task AddOrUpdateToken(TokenInput token)
    {
        await _tokenTableClient.CreateIfNotExistsAsync();

        var user = GetUserClaim(token.IdToken);
        var expireIn = ToUnixTimeSeconds(token.ExpiresIn);
        var tokenEntity = await GetTokenFromStorage(user);

        if (tokenEntity.HasValue)
        {
            await UpdateTokenInStorage(tokenEntity.Value.Update(
                accessToken: token.AccessToken,
                refreshToken: token.RefreshToken,
                expireIn: expireIn));
        }
        else
        {
            await AddTokenToStorage(TokenEntity.Create(
                accessToken: token.AccessToken,
                refreshToken: token.RefreshToken,
                expireIn: expireIn,
                user: user));
        }
    }

    public Task<TokenEntity> GetTokenAndRefreshIfNeed()
        => GetTokenAndRefreshIfNeed(_httpContextAccessor.GetSignedInUser());

    public async Task<TokenEntity> GetTokenAndRefreshIfNeed(string user)
    {
        var tokenEntity = await GetTokenFromStorage(user);
        if (!tokenEntity.HasValue)
            throw new ArgumentNullException(nameof(tokenEntity));

        if (TokenValid(tokenEntity.Value))
            return tokenEntity.Value;

        var tokenResponse = await RefreshToken(tokenEntity.Value);

        var expireIn = ToUnixTimeSeconds(tokenResponse.ExpiresIn);

        await UpdateTokenInStorage(tokenEntity.Value.Update(
            accessToken: tokenResponse.AccessToken,
            refreshToken: tokenResponse.RefreshToken,
            expireIn: expireIn));

        return tokenEntity.Value;
    }

    private async Task<TokenResponse> RefreshToken(TokenEntity tokenEntity)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/common/oauth2/v2.0/token")
        {
            Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _openIdConnectOptions.ClientId!),
                new("scope", string.Join(",", Constants.AuthenticationScopes.ToArray())),
                new("refresh_token", tokenEntity.RefreshToken),
                new("grant_type", GrantType),
                new("client_secret", _openIdConnectOptions.ClientSecret!)
            })
        };

        var httpResponse = await _httpClientFactory
            .CreateClient(Constants.HttpAuthenticationClientName)
            .SendAsync(httpRequest);

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new InvalidOperationException(body);

        return JsonConvert.DeserializeObject<TokenResponse>(body)
               ?? throw new InvalidOperationException(Constants.NullResponseMessage);
    }

    private Task<NullableResponse<TokenEntity>> GetTokenFromStorage(string user)
        => _tokenTableClient.GetEntityIfExistsAsync<TokenEntity>(partitionKey: user, rowKey: user);

    private Task UpdateTokenInStorage(TokenEntity tokenEntity)
        => _tokenTableClient.UpdateEntityAsync(tokenEntity, ETag.All);

    private Task AddTokenToStorage(TokenEntity tokenEntity)
        => _tokenTableClient.AddEntityAsync(tokenEntity);

    private static bool TokenValid(TokenEntity token)
        => token.ExpireIn > DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeSeconds();

    private static string GetUserClaim(string idToken)
    {
        var claims = GetClaims(idToken);
        return claims.First(c => c.Type == Constants.UserClaimName).Value;
    }

    private static IEnumerable<Claim> GetClaims(string idToken)
    {
        var jwtSecurityToken = Handler.ReadJwtToken(idToken);
        return jwtSecurityToken.Claims.ToList();
    }

    private static long ToUnixTimeSeconds(long expiresIn)
        => DateTimeOffset.UtcNow.AddSeconds(expiresIn - 120).ToUnixTimeSeconds();
}