// <copyright file="GeocodeResponseDto.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    public class GeocodeResponseDto
    {
        public GeocodeResponseDto(GeocodeStatus responseStatus)
        {
            this.ResponseStatus = responseStatus;
        }

        public List<GeocodeLocationDto> Locations { get; set; } = new List<GeocodeLocationDto>();

        public GeocodeStatus ResponseStatus { get; set; }
    }
}
