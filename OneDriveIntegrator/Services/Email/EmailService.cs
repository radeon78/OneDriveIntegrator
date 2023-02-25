using Microsoft.Extensions.Options;
using OneDriveIntegrator.Services.Email.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OneDriveIntegrator.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly SendGridClient _sendGridClient;

    private const string Subject = "[OneDrive Integrator POC] Something changed in folder";

    public EmailService(IOptions<EmailOptions> options)
    {
        _emailOptions = options.Value;
        _sendGridClient = new SendGridClient(_emailOptions.ApiKey);
    }

    public async Task Send(EmailInput emailInput)
    {
        if (!_emailOptions.Enabled)
            return;

        var from = new EmailAddress(_emailOptions.From, "OneDrive Integrator");
        var subject = $"{Subject} {emailInput.FolderName}";
        var to = new EmailAddress(emailInput.EmailTo);
        var message = MailHelper.CreateSingleEmail(from, to, subject, emailInput.TextContent, string.Empty);

        await _sendGridClient.SendEmailAsync(message);
    }
}