namespace OneDriveIntegrator.Common;

public static class Constants
{
    public const string UserClaimName = "preferred_username";

    public const string GraphClientName = nameof(GraphClientName);

    public const string AuthClientName = nameof(AuthClientName);

    public static readonly string[] Scopes =
        { "openid", "profile", "email", "offline_access", "Files.ReadWrite.AppFolder", "Files.Read.All" };
}