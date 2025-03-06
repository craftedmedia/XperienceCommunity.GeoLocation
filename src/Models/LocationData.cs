using IPinfo.Models;

using MaxMind.GeoIP2.Responses;

using Microsoft.AspNetCore.Http;

using XperienceCommunity.GeoLocation.Utilities;

namespace XperienceCommunity.GeoLocation.Models;

/// <summary>
/// Location model.
/// </summary>
public class LocationData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationData"/> class.
    /// </summary>
    /// <param name="response"><see cref="CityResponse"/> to populate location from.</param>
    public LocationData(CityResponse response)
    {
        CountryCode = response.Country.IsoCode;
        Country = response.Country.Name;
        RegionCode = response.MostSpecificSubdivision.IsoCode;
        Region = response.MostSpecificSubdivision.Name;
        City = response.City.Name;
        PostCode = response.Postal.Code;
        Latitude = response.Location.Latitude;
        Longitude = response.Location.Longitude;
        TimeZone = response.Location.TimeZone;
        MetroCode = response.Location.MetroCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationData"/> class.
    /// </summary>
    /// <param name="response"><see cref="IPResponse"/> to populate location from.</param>
    public LocationData(IPResponse response)
    {
        CountryCode = response.Country;
        Country = response.CountryName;
        Region = response.Region;
        City = response.City;
        PostCode = response.Postal;
        TimeZone = response.Timezone;

        if (double.TryParse(response.Latitude, out double lat))
        {
            Latitude = lat;
        }

        if (double.TryParse(response.Longitude, out double lng))
        {
            Longitude = lng;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationData"/> class.
    /// </summary>
    /// <param name="cfRequest"><see cref="HttpRequest"/> to populate location from.</param>
    public LocationData(HttpRequest cfRequest)
    {
        CountryCode = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-ipcountry");
        RegionCode = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-region-code");
        Region = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-region");
        City = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-ipcity");
        PostCode = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-postal-code");
        TimeZone = RequestHelper.ParseHeaderValue(cfRequest.Headers, "cf-timezone");
        Latitude = RequestHelper.ParseHeaderValue<double>(cfRequest.Headers, "cf-iplatitude");
        Longitude = RequestHelper.ParseHeaderValue<double>(cfRequest.Headers, "cf-iplongitude");
    }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the region code.
    /// </summary>
    public string? RegionCode { get; set; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the post code.
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the location time zone.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the metro code.
    /// </summary>
    public int? MetroCode { get; set; }
}
