using OneDriveIntegrator.Services.Graph.Models;

namespace OneDriveIntegrator.Services.Graph;

public interface IGraphClient
{
    Task<Items> GetRootChildren();

    Task<Items> GetItemChildren(string id);

    Task<Details> GetItemDetails(string id);

    Task<Details> GetItemDetailsOffline(string id, string accessToken);
}