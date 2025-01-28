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
    public void MapContactData(ContactInfo currentContact)
    {
        // No-op.
    }
}
