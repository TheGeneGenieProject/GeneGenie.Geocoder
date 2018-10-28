// <copyright file="AddressLookupStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// The status of an address lookup.
    /// </summary>
    public enum AddressLookupStatus
    {
        /// <summary>
        /// All available geocoders were queried and a mix of non success results were returned.
        /// If this happens a lot, we need to see why.
        /// </summary>
        MultipleIssues = 101,

        /// <summary>
        /// Geocoder or network error, the operation might work if you try it again (maybe keep a count of the number of tried to avoid an indefinite loop).
        /// </summary>
        TemporaryGeocodeError = 102,

        /// <summary>
        /// Address could not be geocoded by any of the geocoding services. You should check the source data and ask for a user to intervene.
        /// </summary>
        PermanentGeocodeError = 103,

        /// <summary>
        /// All available geocoders were queried (some may be offline) and none of them
        /// returned data.
        /// </summary>
        ZeroResults = 104,

        /// <summary>
        /// Address was successfully geocoded.
        /// </summary>
        Geocoded = 105,
    }
}
