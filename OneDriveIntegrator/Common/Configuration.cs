using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace OneDriveIntegrator.Common;

public static class Configuration
{
    public static string GetOpenIdConnectConfigurationValue(IConfiguration configuration, string key)
        => configuration.GetSection($"{OpenIdConnectDefaults.AuthenticationScheme}:{key}").Value;
}