namespace OneDriveIntegrator.Services.Email.Models;

public class EmailInput
{
    public EmailInput(string emailTo, string textContent, string folderName)
    {
        EmailTo = emailTo;
        TextContent = textContent;
        FolderName = folderName;
    }

    public string EmailTo { get; }

    public string FolderName { get; }

    public string TextContent { get; }
}