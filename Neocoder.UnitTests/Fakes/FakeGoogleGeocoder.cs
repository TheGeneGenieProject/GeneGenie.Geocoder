// <copyright file="FakeGoogleGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.Fakes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Neocoder.Dto;
    using Neocoder.Interfaces;
    using Neocoder.Models.Geo;
    using Neocoder.Services;

    public class FakeGoogleGeocoder : FakeGeocoderBase, IGeocoder
    {
        public GeocoderNames GeocoderId { get => GeocoderNames.Google; }

        public Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var requiredStatusList = ExtractStatusFromAddress(geocodeRequest.Address);
            var requiredStatus = requiredStatusList[GeocoderNames.Google];

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
