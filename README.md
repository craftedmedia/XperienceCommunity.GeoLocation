# XperienceCommunity.GeoLocation
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Description
This package provides basic Geo location functionality by IP address. It supports automatic mapping of Marketing Contact data by location, and has options for configuring alternate Geo location providers.

### Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 30.0.0         | 1.0.0           |

### Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.kentico.com)

### Other requirements
Geo location City/Asn database if using [MaxMind](https://www.maxmind.com/en/home) or an access token if using [IPinfo](https://ipinfo.io/).

### Package Installation

Install the `XperienceCommunity.GeoLocation` package via nuget or run:

```
Install-Package XperienceCommunity.GeoLocation
```
From package manager console.

## Quick Start

### Configuration
Include the following section within your `appsettings.json` file:

```
 "XperienceGeoLocation": {
   "Provider": "MaxMind"
}
```

### Register services
Add the following code to your  `Program.cs`  file:

```
var builder = WebApplication.CreateBuilder(args);

// ...

builder.Services.AddXperienceGeoLocation(builder.Configuration);
```
And include  `UseXperienceGeoLocation()`  before  `app.Run()`:

```
app.UseXperienceGeoLocation();
```

And that should be enough to get going. Read on for more info.

## Full Instructions
### Providers
This package allows for Geo location via [MaxMind](https://www.maxmind.com/en/home) or [IPinfo](https://ipinfo.io/) services. You'll need to provide either a free or paid City/Asn database if using `MaxMind`, or a free or paid Access Token if using `IPinfo`.

Read on for details on configuring each provider.

### MaxMind provider

To use `MaxMind` specify it as the designated provider via `appsettings.json`:

```
"XperienceGeoLocation": {
  "Provider": "MaxMind"
}
```
And provide a City database (required) and optionally, an Asn database in the following filesystem location:
```
~/App_Data/MaxMind/Db/[DB GOES HERE]
```

You can obtain a MaxMind db by signing up here: https://www.maxmind.com/en/geolite2/signup

Once you've signed up, download a free (or paid) database and place it in the aforementioned directory.

#### Deployment
You must ensure that the MaxMind databases are copied to the output directory when deploying, e.g edit csproj to include:

```
<ItemGroup>
  <None Update="App_Data\MaxMind\Db\GeoLite2-ASN.mmdb">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
  <None Update="App_Data\MaxMind\Db\GeoLite2-City.mmdb">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```
#### Paid databases
If you've downloaded a paid database, you may need to configure the database name(s) by passing these in as options:
```
"XperienceGeoLocation": {
  "Provider": "MaxMind",
  "MaxMind": {
    "MaxMindGeoIPCityDbFileName": "NAME OF CITY DATABASE.mmdb",
    "MaxMindGeoIPAsnDbFileName": "NAME OF ASN DATABASE.mmdb"
  }
}
```
This overrides the default (free db) names.

### IPinfo provider
To use the `IPinfo` service, specify it as the designated provider via `appsettings.json`:

```
"XperienceGeoLocation": {
  "Provider": "IPinfo"
}
```
And provide an `AccessToken` in the provider settings:
```
"XperienceGeoLocation": {
  "Provider": "IPinfo",
  "IPinfo": {
    "AccessToken": "ACCESS TOKEN GOES HERE"
  }
}
```

You can obtain an access token by signing up for a free account at [IPinfo](https://ipinfo.io/), and then generate an [access token](https://ipinfo.io/account/token) to be used.

### Geo location service
The package provides an `IGeoLocation` service which can be used to retrieve location data for the current user based on IP address.

Example:

```
public class HomePageController : Controller
{
    private readonly IGeoLocationService geoLocationService;

    public HomePageController(IGeoLocationService geoLocationService)
    {
        this.geoLocationService = geoLocationService;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch location data
        var location = await this.geoLocationService.GetCurrentLocationAsync();

        // Fetch organization data
        var org = await this.geoLocationService.GetOrganizationAsync();

        return new TemplateResult();
    }
}
```

These service methods provide an optional `ip` address parameter which can be used to override the IP address of the current user/request, e.g:

```
var location = await this.geoLocationService.GetCurrentLocationAsync("127.0.0.1");
```

### Map Geo location data to contacts
The package automatically maps Geo location data to the current marketing contact, setting the following fields:

- Country
- State*
- City
- Zip
- Organization / Company**

<sub>* Only when using `MaxMind`</sub>
<sub>** If using `MaxMind`, only when providing an Asn Db</sub>

Note that mapping of contact data is **only performed if the contact has accepted at *least* `Visitor` level cookies**. If the cookie level is below `Visitor` then the system will not attempt to map location data.

You can disable this feature by setting `UseGeoLocationForContacts` to false in `appsettings.json`, e.g:
```
"XperienceGeoLocation": {
  "Provider": "MaxMind",
  "UseGeoLocationForContacts": false
}
```
#### Re-mapping of contacts
Note that **mapping of location data happens only once for a given contact**, once it has been mapped, location data **will not be re-mapped** on subsequent visits e.g. from a different geographical location.

Whether the contact is mapped is controlled by a custom `XperienceGeoLocation_IsContactMapped` field which is stored against the `OM.Contact` class. You can force re-mapping of all contacts by deleting this field and letting the system recreate it.

### Custom Contact mapping logic
It's possible to inject your own mapping logic into the Contact mapping middleware. This will be useful in situations where you need to map additional Contact data (e.g. custom fields), or change the default mapping behaviour.

To do this, you must provide an implementation of `ICustomContactMappingProvider`:

```
public class CustomContactMappingProvider : ICustomContactMappingProvider
{
    private readonly IGeoLocationService geoLocationService;

    public CustomContactMappingProvider(IGeoLocationService geoLocationService) => this.geoLocationService = geoLocationService;
   
    public async Task MapContactDataAsync(ContactInfo currentContact)
    {
        // Do something with the current contact and location data...
        var location = await this.geoLocationService.GetCurrentLocationAsync();

        currentContact.SetValue("ContactTimeZone", location.TimeZone);
    }
}
```

And then register it as a scoped service in your DI container:

`builder.Services.AddScoped<ICustomContactMappingProvider, CustomContactMappingProvider>();`

The custom `MapContactDataAsync` method will now be called as part of the contact mapping pipeline.

### Full configuration
See below for a list of all possible configuration options:

| Option               | Description                                                                                                                                                                 | Example                                                           | Default    |
| -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | ---------- |
| `Provider` | Specify the desired geo location provider. Possible values are `MaxMind` or `IPinfo`.                                                                                                                             | `"IPinfo"`                                                 | `"MaxMind"`
| `UseGeoLocationForContacts` | A value indicating whether to perform automatic mapping of geo location data to the current marketing contact.                                                                                                                            | `"false"`                                                 | `"true"`
| `ContactGeoLocationSuffix` | Provides a suffix which is appended to any geo located field values against the marketing contact.                                                                                                                     | `"(FooSuffix)"`                                                 | `"(Geolocation)"`
| `LocalIpOverride`| Allows for overriding the IP address of the current user. Useful for local testing purposes.                                                                                                                            | `"127.0.0.1"`                                                 | `null`
| `MaxMind.MaxMindGeoIPCityDbFileName`| Allows overriding of the default IP city database name. May be required if using a paid database.                                                                                                                            | `"GeoIP2-City.mmdb"`                                                 | `GeoLite2-City.mmdb`
| `MaxMind.MaxMindGeoIPAsnDbFileName` | Allows overriding of the default IP Asn database name. May be required if using a paid database.                                                                                                                            | `"GeoIP2-Asn.mmdb"`                                                 | `GeoLite2-ASN.mmdb`
| `IPinfo.AccessToken` | Specifies the access token used to authenticate with `IPinfo` services.                                                                                                                          | `"12345abcdef"`                                                 | `null`

### IPinfo rate-limiting
`IPinfo` provides 50k requests per month on their free plan however you will be rate-limited if you exceed this quota.

Requests to `IPinfo` services are cached in memory to minimize the number of calls made, however if you expect to exceed the monthly quota then you should consider using one of their  [paid plans](https://ipinfo.io/pricing), or choosing the `MaxMind` provider with free City/Asn databases instead.


## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

