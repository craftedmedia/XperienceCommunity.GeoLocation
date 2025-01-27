using System.Net;

using CMS.Helpers;

using Microsoft.AspNetCore.Http;

namespace XperienceCommunity.GeoLocation.Utilities;

/// <summary>
/// IP Address helper.
/// </summary>
public class IPAddressHelper
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly XperienceCommunityGeoLocationOptions geoLocationOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="IPAddressHelper"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Instance of <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="geoLocationOptions">Instance of <see cref="XperienceCommunityGeoLocationOptions"/>.</param>
    public IPAddressHelper(IHttpContextAccessor httpContextAccessor, XperienceCommunityGeoLocationOptions geoLocationOptions)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.geoLocationOptions = geoLocationOptions;
    }

    /// <summary>
    /// Gets the current users IP address, accounting for forwarded-for header.
    /// </summary>
    /// <returns>Current user IP address.</returns>
    public string GetCurrentIP()
    {
        if (!string.IsNullOrEmpty(geoLocationOptions.LocalIpOverride))
        {
            return geoLocationOptions.LocalIpOverride!;
        }

        string? forwardedForHeader = httpContextAccessor.HttpContext?.Request.Headers["X-FORWARDED-FOR"].FirstOrDefault(header => header != "::1");

        if (string.IsNullOrEmpty(forwardedForHeader))
        {
            return RequestContext.UserHostAddress;
        }

        string[]? ipAddresses = forwardedForHeader.Split([","], StringSplitOptions.RemoveEmptyEntries);

        if (ipAddresses.Length == 0)
        {
            return RequestContext.UserHostAddress;
        }

        string ip = ipAddresses[0];
        string[] parts = ip.Split(':');

        if (parts.Length == 0)
        {
            return RequestContext.UserHostAddress;
        }

        if (IPAddress.TryParse(parts[0], out var parsedIPAddress) && parsedIPAddress is not null)
        {
            RequestContext.UserHostAddress = parsedIPAddress.ToString();
        }

        return RequestContext.UserHostAddress;
    }
}
