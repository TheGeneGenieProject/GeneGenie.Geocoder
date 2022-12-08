// <copyright file="GeocodeLocationDto.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    /// <summary>
    /// A geocoded address lookup response.
    /// </summary>
    public class GeocodeLocationDto
    {
        /// <summary>
        /// A bounding box around the address found.
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
