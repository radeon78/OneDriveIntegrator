using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using OneDriveIntegrator.Services.Notification.Models;

namespace OneDriveIntegrator.Binders;

public class NotificationBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var stream = bindingContext.HttpContext.Request.Body;
        string body;
        using (var reader = new StreamReader(stream))
            body = await reader.ReadToEndAsync();

        var notification = JsonConvert.DeserializeObject<NotificationRequest>(body);
        bindingContext.Result = ModelBindingResult.Success(notification);
    }
}