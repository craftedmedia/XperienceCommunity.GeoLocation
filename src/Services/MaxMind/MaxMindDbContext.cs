using MaxMind.GeoIP2;

using XperienceCommunity.GeoLocation.Utilities.MaxMind;

namespace XperienceCommunity.GeoLocation.Services.MaxMind;

/// <summary>
/// MaxMind database context.
/// </summary>
internal class MaxMindDbContext : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxMindDbContext"/> class.
    /// </summary>
    /// <param name="geoLocationOptions">Instance of the <see cref="XperienceCommunityGeoLocationOptions"/>.</param>
    public MaxMindDbContext(XperienceCommunityGeoLocationOptions geoLocationOptions)
    {
        CityDatabaseReader = GetCityDb(geoLocationOptions);
        AsnDatabaseReader = GetAsnDb(geoLocationOptions);
    }

    private DatabaseReader? GetCityDb(XperienceCommunityGeoLocationOptions geoLocationOptions)
    {
        string? cityDbPath = DbPathHelper.GetDatabaseFilePath(geoLocationOptions.MaxMind.MaxMindGeoIPCityDbFileName);

        if (!File.Exists(cityDbPath))
        {
            throw new FileNotFoundException($"Could not find a required City DB under the path '{cityDbPath}'. Please ensure that you have added a City database to your project.", geoLocationOptions.MaxMind.MaxMindGeoIPCityDbFileName);
        }

        return new DatabaseReader(cityDbPath);
    }

    private DatabaseReader? GetAsnDb(XperienceCommunityGeoLocationOptions geoLocationOptions)
    {
        string? asnDbPath = DbPathHelper.GetDatabaseFilePath(geoLocationOptions.MaxMind.MaxMindGeoIPAsnDbFileName);

        if (!File.Exists(asnDbPath))
        {
            return null;
        }

        return new DatabaseReader(asnDbPath);
    }

    /// <summary>
    /// Gets the max mind city database reader.
    /// </summary>
    public DatabaseReader? CityDatabaseReader { get; }

    /// <summary>
    /// Gets the max mind asn database reader.
    /// </summary>
    public DatabaseReader? AsnDatabaseReader { get; }

    public void Dispose()
    {
        CityDatabaseReader?.Dispose();
        AsnDatabaseReader?.Dispose();
    }
}
