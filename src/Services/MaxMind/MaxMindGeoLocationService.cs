using CMS.Helpers;

using XperienceCommunity.GeoLocation.Models;
using XperienceCommunity.GeoLocation.Utilities;

namespace XperienceCommunity.GeoLocation.Services.MaxMind;

/// <summary>
/// MaxMind geo location service.
/// </summary>
internal class MaxMindGeoLocationService : IGeoLocationService
{
    private readonly IProgressiveCache progressiveCache;
    private readonly IPAddressHelper ipAddressHelper;
    private readonly MaxMindDbContext maxMindDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxMindGeoLocationService"/> class.
    /// </summary>
    /// <param name="progressiveCache">Instance of <see cref="IProgressiveCache"/>.</param>
    /// <param name="ipAddressHelper">Instance of <see cref="IPAddressHelper"/>.</param>
    /// <param name="maxMindDbContext">Instance of <see cref="MaxMindDbContext"/>.</param>
    public MaxMindGeoLocationService(IProgressiveCache progressiveCache, IPAddressHelper ipAddressHelper, MaxMindDbContext maxMindDbContext)
    {
        this.progressiveCache = progressiveCache;
        this.ipAddressHelper = ipAddressHelper;
        this.maxMindDbContext = maxMindDbContext;
    }

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    public LocationData? GetCurrentLocation(string? ip = null)
    {
        if (maxMindDbContext.CityDatabaseReader is null)
        {
            return null;
        }

        ip ??= ipAddressHelper.GetCurrentIP();

        return progressiveCache.Load(
           (cs) =>
           {
               if (!maxMindDbContext.CityDatabaseReader.TryCity(ip, out var response) || response is null)
               {
                   cs.Cached = false;
                   cs.CacheMinutes = 0;

                   return null;
               }

               return new LocationData(response);
           },
           new CacheSettings(cacheMinutes: 20, nameof(MaxMindGeoLocationService), nameof(this.GetCurrentLocation), ip));
    }

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
    public OrganizationData? GetOrganization(string? ip = null)
    {
        if (maxMindDbContext.AsnDatabaseReader is null)
        {
            return null;
        }

        ip ??= ipAddressHelper.GetCurrentIP();

        return progressiveCache.Load(
           (cs) =>
           {
               if (!maxMindDbContext.AsnDatabaseReader.TryAsn(ip, out var response) || response is null)
               {
                   cs.Cached = false;
                   cs.CacheMinutes = 0;

                   return null;
               }

               return new OrganizationData(response);
           },
           new CacheSettings(cacheMinutes: 20, nameof(MaxMindGeoLocationService), nameof(this.GetOrganization), ip));
    }
}
