// <copyright file="FakeBingGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.UnitTests.Fakes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Dto;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models.Geo;
    using GeneGenie.Geocoder.Services;

    public class FakeBingGeocoder : FakeGeocoderBase, IGeocoder
    {
        public GeocoderNames GeocoderId { get => GeocoderNames.Bing; }

        public Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var requiredStatusList = ExtractStatusFromAddress(geocodeRequest.Address);
            var requiredStatus = requiredStatusList[GeocoderNames.Bing];

            return Task.FromResult(new GeocodeResponseDto(requiredStatus)
            {
                Locations = new List<GeocodeLocationDto>
                {
                    new GeocodeLocationDto { FormattedAddress = "Fake result" },
                },
            });
        }
    }
}
