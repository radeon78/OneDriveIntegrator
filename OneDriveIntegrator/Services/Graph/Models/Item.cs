namespace OneDriveIntegrator.Services.Graph.Models;

public class Item
{
    public string Id { get; init; } = default!;

    public string Name { get; init; } = default!;

    public long Size { get; init; }

    public Folder? Folder { get; init; }

    public File? File { get; init; }

    public bool IsFile() => File != null;
}