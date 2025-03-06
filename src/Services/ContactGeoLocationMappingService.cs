using CMS.ContactManagement;
using CMS.Helpers;

namespace XperienceCommunity.GeoLocation.Services;

/// <summary>
/// Provides geo location mapping for the current contact.
/// </summary>
internal class ContactGeoLocationMappingService
{
    private readonly IGeoLocationService geoLocationService;
    private readonly XperienceCommunityGeoLocationOptions geoLocationOptions;
    private readonly IProgressiveCache progressiveCache;
    private readonly CountryInfoLookupService countryInfoLookupService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactGeoLocationMappingService"/> class.
    /// </summary>
    /// <param name="geoLocationService">Instance of the <see cref="IGeoLocationService"/>.</param>
    /// <param name="geoLocationOptions">Instance of the <see cref="XperienceCommunityGeoLocationOptions"/>.</param>
    /// <param name="progressiveCache">Instance of the <see cref="IProgressiveCache"/>.</param>
    /// <param name="countryInfoLookupService">Instance of the <see cref="CountryInfoLookupService"/>.</param>
    public ContactGeoLocationMappingService(IGeoLocationService geoLocationService, XperienceCommunityGeoLocationOptions geoLocationOptions, IProgressiveCache progressiveCache, CountryInfoLookupService countryInfoLookupService)
    {
        this.geoLocationService = geoLocationService;
        this.geoLocationOptions = geoLocationOptions;
        this.progressiveCache = progressiveCache;
        this.countryInfoLookupService = countryInfoLookupService;
    }

    /// <summary>
    /// Maps Geo location fields to the provided contact.
    /// </summary>
    /// <param name="contact">Contact to map geo location fields to.</param>
    public void MapGeoLocationFieldsToContact(ContactInfo contact)
    {
        if (contact is null)
        {
            return;
        }

        var location = geoLocationService.GetCurrentLocation();

        if (location is null)
        {
            return;
        }

        var countryData = countryInfoLookupService.GetKenticoCountryData(location.CountryCode);
        int stateId = countryInfoLookupService.GetStateId(location.RegionCode);

        contact.ContactCountryID = countryData?.CountryID ?? -1;
        contact.ContactStateID = stateId;

        contact.ContactCity = AppendGeoLocationSuffix(location.City);
        contact.ContactZIP = AppendGeoLocationSuffix(location.PostCode);
    }

    /// <summary>
    /// Maps Geo organization fields to the provided contact.
    /// </summary>
    /// <param name="contact">Contact to map geo organization fields to.</param>
    public void MapGeoOrganizationFieldsToContact(ContactInfo contact)
    {
        if (contact is null)
        {
            return;
        }

        var organization = geoLocationService.GetOrganization();

        if (organization is null)
        {
            return;
        }

        contact.ContactCompanyName = AppendGeoLocationSuffix(organization.OrganizationName);
    }

    private string AppendGeoLocationSuffix(string? data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(geoLocationOptions.ContactGeoLocationSuffix))
        {
            return data;
        }

        return string.Concat(data, " ", geoLocationOptions.ContactGeoLocationSuffix);
    }
}
