using Azure;
using Azure.Data.Tables;

namespace OneDriveIntegrator.Services.Token.Models;

public class TokenEntity : ITableEntity
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;

    public long ExpireIn { get; set; }

    public string PartitionKey { get; set; } = default!;

    public string RowKey { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; } = default!;

    public ETag ETag { get; set; } = default!;

    public static TokenEntity Create(string accessToken, string refreshToken, long expireIn, string user)
        => new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpireIn = expireIn,
            RowKey = user,
            PartitionKey = user
        };

    public TokenEntity Update(string accessToken, string refreshToken, long expireIn)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpireIn = expireIn;

        return this;
    }
}