// <copyright file="LocationPair.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// A pair of coordinates that identify a point on the earths surface.
    /// </summary>
    public class LocationPair
    {
        /// <summary>
        /// The latitude of this location.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// The longitude of this location.
        /// </summary>
        public double Lng { get; set; }
    }
}
