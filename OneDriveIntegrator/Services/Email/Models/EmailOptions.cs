namespace OneDriveIntegrator.Services.Email.Models;

public class EmailOptions
{
    public static string OptionSectionName = "SendGrid";

    public string ApiKey { get; set; } = default!;

    public string From { get; set; } = default!;

    public bool Enabled { get; set; }
}