using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OneDriveIntegrator.Models;
using OneDriveIntegrator.Services.Graph;
using OneDriveIntegrator.Services.Subscription;

namespace OneDriveIntegrator.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index(
        string id,
        [FromServices] IGraphClient graphClient)
        => string.IsNullOrEmpty(id)
            ? View(await graphClient.GetRootChildren())
            : View(await graphClient.GetItemChildren(id));

    public async Task<IActionResult> Details(
        string id,
        [FromServices] IGraphClient graphClient,
        [FromServices] ISubscriptionService subscriptionService)
    {
        var details = await graphClient.GetItemDetails(id);
        var subscription = await subscriptionService.GetSubscription(id);

        var response = new DetailsViewModel(
            details: details,
            subscriptionEnabled: subscription != null);

        return details.IsFile()
            ? View("FileDetails", response)
            : View("FolderDetails", response);
    }

    public async Task<IActionResult> Subscribe(
        string id,
        [FromServices] ISubscriptionService subscriptionService)
    {
        await subscriptionService.Subscribe(id);
        return RedirectToAction("Details", new { id });
    }
    
    public async Task<IActionResult> Unsubscribe(
        string id,
        [FromServices] ISubscriptionService subscriptionService)
    {
        await subscriptionService.Unsubscribe(id);
        return RedirectToAction("Details", new { id });
    }

    public IActionResult Error() => View(new ErrorViewModel
        { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}