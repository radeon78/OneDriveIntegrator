using OneDriveIntegrator.Services.MicrosoftGraph.Models;

namespace OneDriveIntegrator.Services.MicrosoftGraph;

public interface IMicrosoftGraphClient
{
    Task<ItemsResponse> GetRootChildren();
    
    Task<DetailsResponse> GetItemDetails(string id);

    Task<ItemsResponse> GetItemChildren(string id);
}