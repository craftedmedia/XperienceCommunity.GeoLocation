using Microsoft.AspNetCore.Http;

using XperienceCommunity.GeoLocation.Models;

namespace XperienceCommunity.GeoLocation.Services.Cloudflare;

/// <summary>
/// Cloudflare geo location service.
/// </summary>
internal class CloudflareGeoLocationService : IGeoLocationService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly CountryInfoLookupService countryInfoLookupService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareGeoLocationService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Instance of the <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="countryInfoLookupService">Instance of the <see cref="CountryInfoLookupService"/>.</param>
    public CloudflareGeoLocationService(IHttpContextAccessor httpContextAccessor, CountryInfoLookupService countryInfoLookupService)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.countryInfoLookupService = countryInfoLookupService;
    }

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    public LocationData? GetCurrentLocation(string? ip = null)
    {
        if (httpContextAccessor.HttpContext?.Request == null)
        {
            return null;
        }

        var location = new LocationData(httpContextAccessor.HttpContext.Request);

        // Need to do an additional lookup to get country name as Cloudflare only provides country code e.g 'GB'.
        var countryData = countryInfoLookupService.GetKenticoCountryData(location.CountryCode);

        if (countryData is null)
        {
            return location;
        }

        location.Country = countryData.CountryDisplayName;

        return location;
    }

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
    public OrganizationData? GetOrganization(string? ip = null) => null; // This information is not provided by Cloudflare.
}
