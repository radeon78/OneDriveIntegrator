using OneDriveIntegrator.Services.Email;
using OneDriveIntegrator.Services.Email.Models;

namespace OneDriveIntegrator.Extensions.DependencyInjection;

public static class EmailCollectionExtensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<EmailOptions>(configuration.GetSection(EmailOptions.OptionSectionName))
            .AddScoped<IEmailService, EmailService>();
}