namespace XperienceCommunity.GeoLocation;

/// <summary>
/// Geo location options.
/// </summary>
public class XperienceCommunityGeoLocationOptions
{
    /// <summary>
    /// Defines geo location provider options.
    /// </summary>
    public enum GeoLocationProvider
    {
        /// <summary>
        /// MaxMind Database geo location.
        /// </summary>
        MaxMind,

        /// <summary>
        /// IP info geo location service.
        /// </summary>
        IPinfo,

        /// <summary>
        /// Cloudflare request headers.
        /// </summary>
        Cloudflare,
    }

    /// <summary>
    /// Gets or sets the configured geo location provider.
    /// </summary>
    public GeoLocationProvider Provider { get; set; } = GeoLocationProvider.MaxMind;

    /// <summary>
    /// Gets or sets a value indicating whether to use geo location to automatically set contact location data.
    /// </summary>
    public bool UseGeoLocationForContacts { get; set; } = true;

    /// <summary>
    /// Gets or sets the geo location suffix string.
    /// </summary>
    public string ContactGeoLocationSuffix { get; set; } = "(Geolocation)";

    /// <summary>
    /// Gets or sets the max mind options.
    /// </summary>
    public MaxMindOptions MaxMind { get; set; } = new MaxMindOptions();

    /// <summary>
    /// Gets or sets the IP info geo location options.
    /// </summary>
    public IPinfoGeoLocationOptions IPinfo { get; set; } = new IPinfoGeoLocationOptions();

    /// <summary>
    /// Gets or sets an optional IP address to use locally during development.
    /// </summary>
    public string? LocalIpOverride { get; set; }

    /// <summary>
    /// MaxMind API options.
    /// </summary>
    public class MaxMindOptions
    {
        /// <summary>
        /// Gets or sets the Max Mind City Db file name.
        /// </summary>
        public string MaxMindGeoIPCityDbFileName { get; set; } = "GeoLite2-City.mmdb";

        /// <summary>
        /// Gets or sets the Max Mind ASN Db file name.
        /// </summary>
        public string MaxMindGeoIPAsnDbFileName { get; set; } = "GeoLite2-ASN.mmdb";
    }

    /// <summary>
    /// IP info Geo location options.
    /// </summary>
    public class IPinfoGeoLocationOptions
    {
        /// <summary>
        /// Gets or sets the IP info access token.
        /// </summary>
        public string? AccessToken { get; set; }
    }
}
