// <copyright file="GeocoderStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// The status of a location and whether it is worth parsing it.
    /// Values less than zero are not worth looking up and are known
    /// bad values. Zero is uninitialised and greater than zero
    /// means it is in the location validation workflow.
    /// </summary>
    public enum GeocoderStatus
    {
        TemporaryGeocodeError = -105,
        RequiresUserIntervention = -104,
        SeemsToBeADate = -103,
        AllNumeric = -102,
        KnownErroneous = -101,
        SkippedAsEmpty = -100,

        /// <summary>
        /// Needs looking up in our geocoding cache, if not found then moves
        /// to <see cref="RequiresGeocoding"/>
        /// </summary>
        RequiresLookup = 100,
        RequiresGeocoding = 101,
        Geocoded = 102,
    }
}
