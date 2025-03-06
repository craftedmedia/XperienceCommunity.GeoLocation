using IPinfo.Models;

using MaxMind.GeoIP2.Responses;

namespace XperienceCommunity.GeoLocation.Models;

/// <summary>
/// Organisation model.
/// </summary>
public class OrganizationData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationData"/> class.
    /// </summary>
    public OrganizationData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationData"/> class.
    /// </summary>
    /// <param name="response"><see cref="AsnResponse"/> to populate organization from.</param>
    public OrganizationData(AsnResponse response) => OrganizationName = response.AutonomousSystemOrganization;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationData"/> class.
    /// </summary>
    /// <param name="response"><see cref="IPResponse"/> to populate organization from.</param>
    public OrganizationData(IPResponse response) => OrganizationName = response.Org;

    /// <summary>
    /// Gets or sets the organization name.
    /// </summary>
    public string? OrganizationName { get; set; }
}
