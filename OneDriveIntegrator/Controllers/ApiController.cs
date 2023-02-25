using Microsoft.AspNetCore.Mvc;
using OneDriveIntegrator.Binders;
using OneDriveIntegrator.Services.Notification;
using OneDriveIntegrator.Services.Notification.Models;

namespace OneDriveIntegrator.Controllers;

[ApiController]
[Route("api")]
public class ApiController : Controller
{
    [HttpPost("webhook-receiver")]
    public async Task<IActionResult> Post(
        [FromQuery] string? validationToken,
        [FromBody, ModelBinder(BinderType = typeof(NotificationBinder))] NotificationRequest? request,
        [FromServices] INotificationService notificationService)
    {
        if (!string.IsNullOrEmpty(validationToken))
            return Ok(validationToken);

        if (!notificationService.Valid(request))
            return BadRequest();
        
        await notificationService.NotifyUserIfContentInSubscribedFolderChanged(request!);
        return Ok();
    }
}