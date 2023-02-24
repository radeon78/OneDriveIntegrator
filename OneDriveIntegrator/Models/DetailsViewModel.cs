using OneDriveIntegrator.Services.Graph.Models;

namespace OneDriveIntegrator.Models;

public class DetailsViewModel
{
    public DetailsViewModel(Details details, bool subscriptionEnabled)
    {
        Details = details;
        SubscriptionEnabled = subscriptionEnabled;
    }

    public Details Details { get; }

    public bool SubscriptionEnabled { get; }
}