using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace XperienceCommunity.GeoLocation.Utilities.MaxMind;

/// <summary>
/// Path utilities.
/// </summary>
internal static class DbPathHelper
{
    /// <summary>
    /// Gets physical file path to database file.
    /// </summary>
    /// <param name="fileName">Db file name.</param>
    /// <returns>Physical file path.</returns>
    public static string? GetDatabaseFilePath(string fileName)
    {
        string? root = ServiceContainer.Instance.GetRequiredService<IWebHostEnvironment>().ContentRootPath;

        if (string.IsNullOrEmpty(root))
        {
            return null;
        }

        return System.IO.Path.Combine(root, "App_Data", "MaxMind", "Db", fileName);
    }
}
