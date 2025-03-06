using IPinfo;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using XperienceCommunity.GeoLocation.Middleware;
using XperienceCommunity.GeoLocation.Providers;
using XperienceCommunity.GeoLocation.Services;
using XperienceCommunity.GeoLocation.Services.Cloudflare;
using XperienceCommunity.GeoLocation.Services.IPinfo;
using XperienceCommunity.GeoLocation.Services.MaxMind;
using XperienceCommunity.GeoLocation.Utilities;

using static XperienceCommunity.GeoLocation.XperienceCommunityGeoLocationOptions;

namespace XperienceCommunity.GeoLocation;

/// <summary>
/// Registers XperienceCommunity.GeoLocation package.
/// </summary>
public static class XperienceCommunityGeoLocation
{
    /// <summary>
    /// Adds Xperience Geo location services.
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">Instance of <see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddXperienceGeoLocation(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("XperienceGeoLocation").Get<XperienceCommunityGeoLocationOptions>() ?? throw new ArgumentNullException("XperienceGeoLocation", "No appsettings section found matching expected 'XperienceGeoLocation' section.");

        services.AddSingleton(options);
        services.AddScoped<IPAddressHelper>();
        services.AddScoped<ContactGeoLocationMappingService>();
        services.AddSingleton<CountryInfoLookupService>();
        services.TryAddScoped<ICustomContactMappingProvider, NoOpContactMappingProvider>();

        switch (options.Provider)
        {
            default:
            case GeoLocationProvider.MaxMind:

                ConfigureMaxMind(services);

                break;

            case GeoLocationProvider.IPinfo:

                ConfigureIPInfo(services, options);

                break;

            case GeoLocationProvider.Cloudflare:

                ConfigureCloudflare(services);

                break;
        }

        ServiceContainer.Instance = services.BuildServiceProvider();

        return services;
    }

    /// <summary>
    /// Uses Xperience Geo location.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/> instance.</param>
    /// <returns><see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseXperienceGeoLocation(this IApplicationBuilder app)
    {
        ContactClassInfoHelper.EnsureContactMappedField();

        app.UseMiddleware<ContactGeoLocationMiddleware>();

        return app;
    }

    private static void ConfigureMaxMind(IServiceCollection services)
    {
        services.AddSingleton<MaxMindDbContext>();
        services.AddScoped<IGeoLocationService, MaxMindGeoLocationService>();
    }

    private static void ConfigureIPInfo(IServiceCollection services, XperienceCommunityGeoLocationOptions options)
    {
        if (string.IsNullOrEmpty(options?.IPinfo?.AccessToken))
        {
            throw new ArgumentNullException(nameof(IPinfoGeoLocationOptions.AccessToken), "An IPinfo access token is required.");
        }

        var client = new IPinfoClient.Builder()
            .AccessToken(options.IPinfo.AccessToken)
            .Build();

        services.AddSingleton(client);
        services.AddScoped<IGeoLocationService, IPinfoGeoLocationService>();
    }

    private static void ConfigureCloudflare(IServiceCollection services) => services.AddScoped<IGeoLocationService, CloudflareGeoLocationService>();
}
