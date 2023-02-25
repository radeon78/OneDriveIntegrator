using OneDriveIntegrator.Services.Notification.Models;

namespace OneDriveIntegrator.Services.Notification;

public interface INotificationService
{
    bool Valid(NotificationRequest? request);
    
    Task NotifyUserIfContentInSubscribedFolderChanged(NotificationRequest notificationRequest);
}