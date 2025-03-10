﻿using CMS.ContactManagement;
using CMS.Helpers;

using Microsoft.AspNetCore.Http;

using XperienceCommunity.GeoLocation.Providers;
using XperienceCommunity.GeoLocation.Services;
using XperienceCommunity.GeoLocation.Utilities;

namespace XperienceCommunity.GeoLocation.Middleware;

/// <summary>
/// Adds Geo located data to the current contact.
/// </summary>
internal class ContactGeoLocationMiddleware
{
    private readonly RequestDelegate next;
    private readonly ICurrentCookieLevelProvider currentCookieLevelProvider;
    private readonly XperienceCommunityGeoLocationOptions geoLocationOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactGeoLocationMiddleware"/> class.
    /// </summary>
    /// <param name="next">Instance of the <see cref="RequestDelegate"/>.</param>
    /// <param name="currentCookieLevelProvider">Instance of the <see cref="ICurrentCookieLevelProvider"/>.</param>
    /// <param name="geoLocationOptions">Instance of the <see cref="XperienceCommunityGeoLocationOptions"/>.</param>
    public ContactGeoLocationMiddleware(RequestDelegate next, ICurrentCookieLevelProvider currentCookieLevelProvider, XperienceCommunityGeoLocationOptions geoLocationOptions)
    {
        this.next = next;
        this.currentCookieLevelProvider = currentCookieLevelProvider;
        this.geoLocationOptions = geoLocationOptions;
    }

    /// <summary>
    /// Handle the invoke async method.
    /// </summary>
    /// <param name="context">Instance of the current <see cref="HttpContext"/>.</param>
    /// <param name="contactGeoLocationMappingService">Instance of the <see cref="ContactGeoLocationMappingService"/>.</param>
    /// <param name="provider">Instance of the <see cref="ICustomContactMappingProvider"/>.</param>
    /// <returns>Task.</returns>
    public async Task InvokeAsync(HttpContext context, ContactGeoLocationMappingService contactGeoLocationMappingService, ICustomContactMappingProvider provider)
    {
        // Mapping contact location not required, skip.
        if (!geoLocationOptions.UseGeoLocationForContacts)
        {
            await next(context);
            return;
        }

        int level = currentCookieLevelProvider.GetCurrentCookieLevel();

        // Don't perform geo location if the users cookie level is not at least visitor.
        if (level < Kentico.Web.Mvc.CookieLevel.Visitor.Level)
        {
            await next(context);
            return;
        }

        var contact = ContactManagementContext.GetCurrentContact();

        if (contact is null)
        {
            await next(context);
            return;
        }

        // Don't perform geo location on manually created contacts.
        if (contact.ContactCreatedInAdministration)
        {
            await next(context);
            return;
        }

        // Don't perform geo location if the contact has already been mapped.
        if (contact.GetBooleanValue(ContactClassInfoHelper.ContactMappedColumnName, false))
        {
            await next(context);
            return;
        }

        // Perform lookup / mappings.
        contactGeoLocationMappingService.MapGeoLocationFieldsToContact(contact);
        contactGeoLocationMappingService.MapGeoOrganizationFieldsToContact(contact);

        // Perform user supplied mappings.
        await provider.MapContactDataAsync(contact);

        contact.SetValue(ContactClassInfoHelper.ContactMappedColumnName, true);
        contact.Update();

        await next(context);
    }
}
