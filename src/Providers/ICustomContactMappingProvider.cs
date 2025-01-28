using CMS.ContactManagement;

namespace XperienceCommunity.GeoLocation.Providers;

/// <summary>
/// Provides custom mapping logic to be injected in the contact geo location mapping middleware.
/// </summary>
public interface ICustomContactMappingProvider
{
    /// <summary>
    /// Maps additional contact data.
    /// </summary>
    /// <param name="currentContact">Instance of the current contact.</param>
    void MapContactData(ContactInfo currentContact);
}
