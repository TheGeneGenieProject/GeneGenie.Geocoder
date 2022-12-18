// <copyright file="ResponseDetail.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    /// <summary>
    /// Holds the status code and descriptive text for the address lookup.
    /// </summary>
    /// <param name="Description">
    /// A textual description of the response to be displayed to the user. Used for
    /// troubleshooting failed address lookups.</param>
    /// <param name="GeocodeStatus">The status of the geocode lookup.</param>
    public record ResponseDetail(string Description, GeocodeStatus GeocodeStatus)
    {
    }
}
