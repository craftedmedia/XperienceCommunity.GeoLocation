using CMS.Helpers;

using MaxMind.GeoIP2;

using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.GeoLocation.Models;
using XperienceCommunity.GeoLocation.Utilities;
using XperienceCommunity.GeoLocation.Utilities.MaxMind;

namespace XperienceCommunity.GeoLocation.Services;

/// <summary>
/// MaxMind geo location service.
/// </summary>
internal class MaxMindGeoLocationService : IGeoLocationService, IDisposable
{
    private static readonly object fileLock = new();
    private static IGeoIP2DatabaseReader? geoIpDatabaseReader;
    private static IGeoIP2DatabaseReader? asnDatabaseReader;
    private readonly IProgressiveCache progressiveCache;
    private readonly IPAddressHelper ipAddressHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxMindGeoLocationService"/> class.
    /// </summary>
    /// <param name="progressiveCache">Instance of <see cref="IProgressiveCache"/>.</param>
    /// <param name="ipAddressHelper">Instance of <see cref="IPAddressHelper"/>.</param>
    public MaxMindGeoLocationService(IProgressiveCache progressiveCache, IPAddressHelper ipAddressHelper)
    {
        this.progressiveCache = progressiveCache;
        this.ipAddressHelper = ipAddressHelper;
    }

    /// <summary>
    /// Gets the Geo IP location database reader.
    /// </summary>
    internal static IGeoIP2DatabaseReader? LocationDatabaseReader
    {
        get
        {
            if (geoIpDatabaseReader == null)
            {
                lock (fileLock)
                {
                    var config = ServiceContainer.Instance.GetRequiredService<XperienceCommunityGeoLocationOptions>();

                    config ??= new XperienceCommunityGeoLocationOptions();

                    string? filePath = DbPathHelper.GetDatabaseFilePath(config.MaxMind.MaxMindGeoIPCityDbFileName);

                    if (string.IsNullOrEmpty(filePath))
                    {
                        throw new ArgumentNullException(nameof(filePath), "Location DB file path invalid.");
                    }

                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Could not find a required City DB under the path '{filePath}'. Please ensure that you have added a City database to your project.", config.MaxMind.MaxMindGeoIPCityDbFileName);
                    }

                    geoIpDatabaseReader ??= new DatabaseReader(filePath);
                }
            }

            return geoIpDatabaseReader;
        }
    }

    /// <summary>
    /// Gets the Geo IP ASN database reader.
    /// </summary>
    internal static IGeoIP2DatabaseReader? AsnDatabaseReader
    {
        get
        {
            if (asnDatabaseReader == null)
            {
                lock (fileLock)
                {
                    var config = ServiceContainer.Instance.GetRequiredService<XperienceCommunityGeoLocationOptions>();

                    config ??= new XperienceCommunityGeoLocationOptions();

                    string? filePath = DbPathHelper.GetDatabaseFilePath(config.MaxMind.MaxMindGeoIPAsnDbFileName);

                    if (string.IsNullOrEmpty(filePath))
                    {
                        throw new ArgumentNullException(nameof(filePath), "Asn DB file path invalid.");
                    }

                    if (asnDatabaseReader is null && File.Exists(filePath))
                    {
                        asnDatabaseReader = new DatabaseReader(filePath);
                    }
                }
            }

            return asnDatabaseReader;
        }
    }

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    public LocationData? GetCurrentLocation(string? ip = null)
    {
        if (LocationDatabaseReader is null)
        {
            return null;
        }

        ip ??= ipAddressHelper.GetCurrentIP();

        return progressiveCache.Load(
           (cs) =>
           {
               if (!LocationDatabaseReader.TryCity(ip, out var response) || response is null)
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
    public OrganizationData? GetOrganizationData(string? ip = null)
    {
        if (AsnDatabaseReader is null)
        {
            return null;
        }

        ip ??= ipAddressHelper.GetCurrentIP();

        return progressiveCache.Load(
           (cs) =>
           {
               if (!AsnDatabaseReader.TryAsn(ip, out var response) || response is null)
               {
                   cs.Cached = false;
                   cs.CacheMinutes = 0;

                   return null;
               }

               return new OrganizationData(response);
           },
           new CacheSettings(cacheMinutes: 20, nameof(MaxMindGeoLocationService), nameof(this.GetOrganizationData), ip));
    }

    /// <summary>
    /// Disposes of MaxMind database resources.
    /// </summary>
    public void Dispose()
    {
        if (LocationDatabaseReader is IDisposable disposeLocationDb)
        {
            disposeLocationDb.Dispose();
        }

        if (AsnDatabaseReader is IDisposable disposeAsnDb)
        {
            disposeAsnDb.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
