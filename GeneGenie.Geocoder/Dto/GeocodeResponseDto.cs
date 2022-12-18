// <copyright file="GeocodeResponseDto.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    /// <summary>
    /// Holds the result of an address geocode passed through one of the geocoder engines.
    /// </summary>
    public class GeocodeResponseDto
    {
        /// <summary>
        /// Creates a new response from the geocoder with the passed status.
        /// </summary>
        /// <param name="responseDetail">The status of the lookup to be returned to the caller.</param>
        public GeocodeResponseDto(ResponseDetail responseDetail)
        {
            this.ResponseDetail = responseDetail;
        }

        /// <summary>
        /// A list of possible locations for the address searched.
        /// </summary>
        public List<GeocodeLocationDto> Locations { get; set; } = new List<GeocodeLocationDto>();

        /// <summary>
        /// The status of the geocode lookup.
        /// </summary>
        public ResponseDetail ResponseDetail { get; set; }
    }
}
