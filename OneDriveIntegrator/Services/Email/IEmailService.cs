using OneDriveIntegrator.Services.Email.Models;

namespace OneDriveIntegrator.Services.Email;

public interface IEmailService
{
    Task Send(EmailInput emailInput);
}