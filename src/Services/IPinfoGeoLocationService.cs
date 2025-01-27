using CMS.Core;

using IPinfo;
using IPinfo.Models;

using XperienceCommunity.GeoLocation.Models;
using XperienceCommunity.GeoLocation.Utilities;

namespace XperienceCommunity.GeoLocation.Services;

/// <summary>
/// IP info geo location service.
/// </summary>
internal class IPinfoGeoLocationService : IGeoLocationService
{
    private readonly IPinfoClient ipInfoClient;
    private readonly IPAddressHelper ipAddressHelper;
    private readonly IEventLogService eventLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="IPinfoGeoLocationService"/> class.
    /// </summary>
    /// <param name="ipInfoClient">Instance of the <see cref="IPinfoClient"/>.</param>
    /// <param name="ipAddressHelper">Instance of <see cref="IPAddressHelper"/>.</param>
    /// <param name="eventLogService">Instance of <see cref="IEventLogService"/>.</param>
    public IPinfoGeoLocationService(IPinfoClient ipInfoClient, IPAddressHelper ipAddressHelper, IEventLogService eventLogService)
    {
        this.ipInfoClient = ipInfoClient;
        this.ipAddressHelper = ipAddressHelper;
        this.eventLogService = eventLogService;
    }

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    public LocationData? GetCurrentLocation(string? ip = null)
    {
        var response = GetResponse(ip);

        if (response is null)
        {
            return null;
        }

        return new LocationData(response);
    }

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    public async Task<LocationData?> GetCurrentLocationAsync(string? ip = null)
    {
        var response = await GetResponseAsync(ip);

        if (response is null)
        {
            return null;
        }

        return new LocationData(response);
    }

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
    public OrganizationData? GetOrganizationData(string? ip = null)
    {
        var response = GetResponse(ip);

        if (response is null)
        {
            return null;
        }

        return new OrganizationData(response);
    }

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
    public async Task<OrganizationData?> GetOrganizationDataAsync(string? ip = null)
    {
        var response = await GetResponseAsync(ip);

        if (response is null)
        {
            return null;
        }

        return new OrganizationData(response);
    }

    private IPResponse? GetResponse(string? ip = null)
    {
        try
        {
            ip ??= ipAddressHelper.GetCurrentIP();

            return ipInfoClient.IPApi.GetDetails(ip);
        }
        catch (HttpRequestException exc) when (exc.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            eventLogService.LogWarning(nameof(IPinfoGeoLocationService), nameof(this.GetResponseAsync), $"Cannot retrieve IP location data due to request throttling imposed by IPinfo service. Error: {exc.Message}");

            return null;
        }
    }

    private async Task<IPResponse?> GetResponseAsync(string? ip = null)
    {
        try
        {
            ip ??= ipAddressHelper.GetCurrentIP();

            return await ipInfoClient.IPApi.GetDetailsAsync(ip);
        }
        catch (HttpRequestException exc) when (exc.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            eventLogService.LogWarning(nameof(IPinfoGeoLocationService), nameof(this.GetResponseAsync), $"Cannot retrieve IP location data due to request throttling imposed by IPinfo service. Error: {exc.Message}");

            return null;
        }
    }
}
