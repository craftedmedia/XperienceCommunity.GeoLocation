using XperienceCommunity.GeoLocation.Models;

namespace XperienceCommunity.GeoLocation.Services;

/// <summary>
/// Retreives geo location data from the configured provider.
/// </summary>
public interface IGeoLocationService
{
    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
    LocationData? GetCurrentLocation(string? ip = null);

    /// <summary>
    /// Gets location data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Location data.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    async Task<LocationData?> GetCurrentLocationAsync(string? ip = null) => GetCurrentLocation(ip);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
    OrganizationData? GetOrganizationData(string? ip = null);

    /// <summary>
    /// Gets organization data for the current user.
    /// </summary>
    /// <param name="ip">Optional IP address. If not provided, uses current request IP.</param>
    /// <returns>Organization data.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    async Task<OrganizationData?> GetOrganizationDataAsync(string? ip = null) => GetOrganizationData(ip);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
