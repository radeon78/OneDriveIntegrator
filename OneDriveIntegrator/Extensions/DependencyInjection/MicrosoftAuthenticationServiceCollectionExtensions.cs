using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Validators;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class MicrosoftAuthenticationServiceCollectionExtensions
{
    private const string DefaultScheme = "Cookies";

    public static IServiceCollection AddMicrosoftAuthenticationService(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services
            .Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority =
                        GetOpenIdConnectConfigurationValue(configuration, nameof(OpenIdConnectOptions.Authority));
                    options.ClientId =
                        GetOpenIdConnectConfigurationValue(configuration, nameof(OpenIdConnectOptions.ClientId));
                    options.ClientSecret =
                        GetOpenIdConnectConfigurationValue(configuration, nameof(OpenIdConnectOptions.ClientSecret));
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.GetAadIssuerValidator(
                        options.Authority,
                        options.Backchannel).Validate;
                })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = DefaultScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(DefaultScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = false;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = DefaultScheme;
                options.SaveTokens = true;
                options.UseTokenLifetime = false;
                options.RequireHttpsMetadata = true;
                options.UsePkce = true;
                
                options.ResponseType = "code id_token";

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.Scope.Add("Files.ReadWrite.AppFolder");
                options.Scope.Add("Files.Read.All");
            });

        if (environment.IsDevelopment())
            return services;

        services.AddDataProtection();

        return services;
    }

    public static IApplicationBuilder UseMicrosoftAuthenticationService(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (context, next) =>
        {
            if (context.User.Identity is not { IsAuthenticated: true })
                await context.ChallengeAsync();
            else
                await next();
        });

        return app;
    }

    private static string GetOpenIdConnectConfigurationValue(IConfiguration configuration, string key)
        => configuration.GetSection($"{OpenIdConnectDefaults.AuthenticationScheme}:{key}").Value;
}