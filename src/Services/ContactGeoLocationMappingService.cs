using CMS.ContactManagement;
using CMS.Globalization;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactGeoLocationMappingService"/> class.
    /// </summary>
    /// <param name="geoLocationService">Instance of the <see cref="IGeoLocationService"/>.</param>
    /// <param name="geoLocationOptions">Instance of the <see cref="XperienceCommunityGeoLocationOptions"/>.</param>
    /// <param name="progressiveCache">Instance of the <see cref="IProgressiveCache"/>.</param>
    public ContactGeoLocationMappingService(IGeoLocationService geoLocationService, XperienceCommunityGeoLocationOptions geoLocationOptions, IProgressiveCache progressiveCache)
    {
        this.geoLocationService = geoLocationService;
        this.geoLocationOptions = geoLocationOptions;
        this.progressiveCache = progressiveCache;
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

        contact.ContactCountryID = GetCountryId(location.CountryCode);
        contact.ContactStateID = GetStateId(location.RegionCode);
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

        var organization = geoLocationService.GetOrganizationData();

        if (organization is null)
        {
            return;
        }

        contact.ContactCompanyName = AppendGeoLocationSuffix(organization.OrganizationName);
    }

    private int GetStateId(string? regionCode)
    {
        if (string.IsNullOrWhiteSpace(regionCode))
        {
            return -1;
        }

        return progressiveCache.Load(
          (cs) =>
          {
              var info = StateInfoProvider.GetStateInfoByCode(regionCode);

              if (info is null)
              {
                  cs.Cached = false;
                  cs.CacheMinutes = 0;
                  return -1;
              }

              return info.StateID;
          },
          new CacheSettings(cacheMinutes: 20, nameof(ContactGeoLocationMappingService), nameof(this.GetStateId), regionCode));
    }

    private int GetCountryId(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return -1;
        }

        return progressiveCache.Load(
          (cs) =>
          {
              var info = CountryInfoProvider.GetCountryInfoByCode(countryCode);

              if (info is null)
              {
                  cs.Cached = false;
                  cs.CacheMinutes = 0;
                  return -1;
              }

              return info.CountryID;
          },
          new CacheSettings(cacheMinutes: 20, nameof(ContactGeoLocationMappingService), nameof(this.GetCountryId), countryCode));
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
