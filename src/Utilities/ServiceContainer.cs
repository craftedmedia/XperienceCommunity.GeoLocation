namespace XperienceCommunity.GeoLocation.Utilities;

/// <summary>
/// Lightweight service locator. Required as we cannot use DI in Kentico modules.
/// </summary>
internal static class ServiceContainer
{
    /// <summary>
    /// Gets or sets the service provider instance.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static IServiceProvider Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
