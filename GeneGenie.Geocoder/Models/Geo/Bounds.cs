// <copyright file="Bounds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// Standard map bounds representing a rectangle returned from one of the geocoders.
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// The top right of the bounding rectangle for this map location.
        /// </summary>
        public LocationPair NorthEast { get; set; }

        /// <summary>
        /// The bottom left of the bounding rectangle for this map location.
        /// </summary>
        public LocationPair SouthWest { get; set; }
    }
}
