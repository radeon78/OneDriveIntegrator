using Microsoft.AspNetCore.Mvc;
using OneDriveIntegrator.Binders;
using OneDriveIntegrator.Services.Subscription;
using OneDriveIntegrator.Services.Subscription.Models;

namespace OneDriveIntegrator.Controllers;

[ApiController]
[Route("api")]
public class ApiController : Controller
{
    [HttpPost("webhook-receiver")]
    public async Task<IActionResult> Post(
        [FromQuery] string? validationToken,
        [FromBody, ModelBinder(BinderType = typeof(NotificationBinder))] Notification? request,
        [FromServices] ISubscriptionService subscriptionService)
    {
        if (!string.IsNullOrEmpty(validationToken))
            return Ok(validationToken);

        if (request == null)
            return BadRequest($"{nameof(validationToken)} and {request} can not be empty");

        await subscriptionService.AddNotification(request);
        return Ok();
    }
}