using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OneDriveIntegrator.Models;
using OneDriveIntegrator.Services.MicrosoftGraph;

namespace OneDriveIntegrator.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index(
        string id,
        [FromServices] IMicrosoftGraphClient microsoftGraphClient)
        => string.IsNullOrEmpty(id)
            ? View(await microsoftGraphClient.GetRootChildren())
            : View(await microsoftGraphClient.GetItemChildren(id));

    public async Task<IActionResult> Details(
        string id,
        [FromServices] IMicrosoftGraphClient microsoftGraphClient)
    {
        var response = await microsoftGraphClient.GetItemDetails(id);
        return response.IsFile()
            ? View("FileDetails", response)
            : View("FolderDetails", response);
    }

    public IActionResult Error() => View(new ErrorViewModel
        { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}