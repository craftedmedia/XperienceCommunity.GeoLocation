using CMS.Globalization;
using CMS.Helpers;

using XperienceCommunity.GeoLocation.Models;

namespace XperienceCommunity.GeoLocation.Services;

/// <summary>
/// Country info lookup service.
/// </summary>
internal class CountryInfoLookupService
{
    private readonly IProgressiveCache progressiveCache;

    public CountryInfoLookupService(IProgressiveCache progressiveCache) => this.progressiveCache = progressiveCache;

    /// <summary>
    /// Gets region state ID from Kentico DB.
    /// </summary>
    /// <param name="regionCode">Region code.</param>
    /// <returns>State ID.</returns>
    public int GetStateId(string? regionCode)
    {
        if (string.IsNullOrWhiteSpace(regionCode))
        {
            return -1;
        }

        return progressiveCache.Load(
          (cs) =>
          {
              var info = StateInfoProvider.GetStateInfoByCode(regionCode);

              if (info is null)
              {
                  cs.Cached = false;
                  cs.CacheMinutes = 0;
                  return -1;
              }

              return info.StateID;
          },
          new CacheSettings(cacheMinutes: 60, nameof(CountryInfoLookupService), nameof(this.GetStateId), regionCode));
    }

    /// <summary>
    /// Gets Kentico Country data from DB.
    /// </summary>
    /// <param name="countryCode">Country code to lookup.</param>
    /// <returns>Kentic Country data.</returns>
    public CountryData? GetKenticoCountryData(string? countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
        {
            return null;
        }

        return progressiveCache.Load(
         (cs) =>
         {
             var info = CountryInfoProvider.GetCountryInfoByCode(countryCode);

             if (info is null)
             {
                 cs.Cached = false;
                 cs.CacheMinutes = 0;
                 return null;
             }

             return new CountryData()
             {
                 CountryID = info.CountryID,
                 CountryDisplayName = info.CountryDisplayName,
             };
         },
         new CacheSettings(cacheMinutes: 60, nameof(CountryInfoLookupService), nameof(this.GetKenticoCountryData), countryCode));
    }
}
