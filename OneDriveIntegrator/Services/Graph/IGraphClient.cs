using OneDriveIntegrator.Services.Graph.Models;

namespace OneDriveIntegrator.Services.Graph;

public interface IGraphClient
{
    Task<Items> GetRootChildren();
    
    Task<Details> GetItemDetails(string id);

    Task<Items> GetItemChildren(string id);
}