// <copyright file="GeocodeResponseLocation.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// A geocoder agnostic result to an address geocode call.
    /// </summary>
    public class GeocodeResponseLocation
    {
        /// <summary>
        /// A latitude and longitude based rectangle defining the map region for the results.
        /// </summary>
        public Bounds Bounds { get; set; }

        /// <summary>
        /// A formatted address as returned from the geocoder service.
        /// This format is for display to the end user as it is abbreviated and therefore lossy.
        /// </summary>
        public string FormattedAddress { get; set; }

        /// <summary>
        /// The map point of the address geocode.
        /// </summary>
        public LocationPair Location { get; set; }
    }
}
