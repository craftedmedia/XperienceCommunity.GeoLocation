using Microsoft.AspNetCore.Http;

namespace XperienceCommunity.GeoLocation.Utilities;

/// <summary>
/// Http Request helper.
/// </summary>
internal class RequestHelper
{
    /// <summary>
    /// Parses <typeparamref name="T"/> header value.
    /// </summary>
    /// <typeparam name="T"><typeparamref name="T"/> type to parse.</typeparam>
    /// <param name="headers">Headers dictionary.</param>
    /// <param name="key">Header key.</param>
    /// <param name="defaultValue">Default value if not found.</param>
    /// <returns>Parsed header value.</returns>
    public static T? ParseHeaderValue<T>(IHeaderDictionary? headers, string key, T? defaultValue = default)
        where T : struct, IParsable<T>
    {
        if (headers is null)
        {
            return defaultValue;
        }

        if (!headers.TryGetValue(key, out var values) || !values.Any())
        {
            return defaultValue;
        }

        string? raw = values[0];

        if (string.IsNullOrEmpty(raw))
        {
            return defaultValue;
        }

        if (!T.TryParse(raw, null, out var value))
        {
            return defaultValue;
        }

        return value;
    }

    /// <summary>
    /// Parses string request header value.
    /// </summary>
    /// <param name="headers">Headers dictionary.</param>
    /// <param name="key">Header key.</param>
    /// <param name="defaultValue">Default value if not found.</param>
    /// <returns>Parsed header value.</returns>
    public static string? ParseHeaderValue(IHeaderDictionary? headers, string key, string? defaultValue = null)
    {
        if (headers is null)
        {
            return defaultValue;
        }

        if (!headers.TryGetValue(key, out var values) || !values.Any())
        {
            return defaultValue;
        }

        return values[0];
    }
}
