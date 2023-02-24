namespace OneDriveIntegrator.Common;

public static class Constants
{
    public const string UserClaimName = "preferred_username";

    public const string HttpGraphClientName = nameof(HttpGraphClientName);

    public const string HttpAuthenticationClientName = nameof(HttpAuthenticationClientName);

    public static readonly string[] AuthenticationScopes =
        { "openid", "profile", "email", "offline_access", "Files.Read.All" };

    public const string NullResponseMessage = "Response is null or empty";
}