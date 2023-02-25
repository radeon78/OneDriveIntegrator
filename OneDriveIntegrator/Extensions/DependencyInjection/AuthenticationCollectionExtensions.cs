using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Validators;
using OneDriveIntegrator.Common;
using OneDriveIntegrator.Services.Token;
using OneDriveIntegrator.Services.Token.Models;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class AuthenticationCollectionExtensions
{
    private const string DefaultScheme = "Cookies";
    private const string CookieName = "OneDriveIntegrator";

    public static IServiceCollection AddAuthenticationService(
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
                options.Cookie.Name = CookieName;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = DefaultScheme;
                options.SaveTokens = false;

                options.ResponseType = "code id_token";

                options.Scope.Clear();
                foreach (var scope in Constants.AuthenticationScopes)
                    options.Scope.Add(scope);

                options.Events.OnTokenResponseReceived = async ctx =>
                {
                    var tokenService = ctx.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                    await tokenService.AddOrUpdateToken(new TokenInput(
                        accessToken: ctx.TokenEndpointResponse.AccessToken,
                        refreshToken: ctx.TokenEndpointResponse.RefreshToken,
                        idToken: ctx.TokenEndpointResponse.IdToken,
                        expiresIn: long.Parse(ctx.TokenEndpointResponse.ExpiresIn)));
                };
            });

        if (environment.IsDevelopment())
            return services;

        services.AddDataProtection();

        return services;
    }

    public static IApplicationBuilder UseAuthenticationService(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (context, next) =>
        {
            if (context.User.Identity is not { IsAuthenticated: true } &&
                !context.Request.Path.StartsWithSegments("/api/webhook-receiver"))
                await context.ChallengeAsync();
            else
                await next();
        });

        return app;
    }

    public static string GetOpenIdConnectConfigurationValue(IConfiguration configuration, string key)
        => configuration.GetSection($"{OpenIdConnectDefaults.AuthenticationScheme}:{key}").Value;
}