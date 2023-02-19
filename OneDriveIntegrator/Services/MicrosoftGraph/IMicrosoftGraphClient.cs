using OneDriveIntegrator.Services.MicrosoftGraph.Models;

namespace OneDriveIntegrator.Services.MicrosoftGraph;

public interface IMicrosoftGraphClient
{
    Task<Items> GetRootChildren();
    
    Task<Details> GetItemDetails(string id);

    Task<Items> GetItemChildren(string id);
}