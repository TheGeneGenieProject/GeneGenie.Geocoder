// <copyright file="LocationPair.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// Output location pair from geocoder, isolated from the API response location data.
    /// </summary>
    public class LocationPair
    {
        /// <summary>
        /// The latitude of this location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of this location.
        /// </summary>
        public double Longitude { get; set; }
    }
}
