using CMS.ContactManagement;

namespace XperienceCommunity.GeoLocation.Providers;

/// <summary>
/// No-op mapping provider. Will be overriden as required.
/// </summary>
internal class NoOpContactMappingProvider : ICustomContactMappingProvider
{
    /// <summary>
    /// Maps additional contact data.
    /// </summary>
    /// <param name="currentContact">Instance of the current contact.</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task MapContactDataAsync(ContactInfo currentContact)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        // No-op.
    }
}
