// <copyright file="AddressLookupStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Models.Geo
{
    /// <summary>
    /// The status of a address lookup and whether it is worth parsing it.
    /// Values less than zero are not worth looking up and are known
    /// bad values. Zero is uninitialised and greater than zero
    /// means it is in the geocoding workflow.
    /// </summary>
    public enum AddressLookupStatus
    {
        SeemsToBeADate = -103,

        AllNumeric = -102,

        KnownErroneous = -101,

        SkippedAsEmpty = -100,

        NotSet = 0,

        /// <summary>
        /// Has passed the basic quality tests and needs looking up.
        /// </summary>
        RequiresLookup = 100,

        [System.Obsolete("Possibly needs deleting as we don't cache any more")]
        RequiresGeocoding = 101,

        /// <summary>
        /// All available geocoders were queried and a mix of non success results were returned.
        /// If this happens a lot, we need to see why.
        /// </summary>
        MultipleIssues = 101,

        TemporaryGeocodeError = 102,

        PermanentGeocodeError = 103,

        /// <summary>
        /// All available geocoders were queried (some may be offline) and none of them
        /// returned data.
        /// </summary>
        ZeroResults = 104,

        Geocoded = 105,
    }
}
