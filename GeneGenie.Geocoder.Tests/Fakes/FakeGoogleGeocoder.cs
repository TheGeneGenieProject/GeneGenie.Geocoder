// <copyright file="FakeGoogleGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    /// <summary>
    /// A fake Google geocoder for unit testing.
    /// </summary>
    internal sealed class FakeGoogleGeocoder : FakeGeocoderBase, IGeocoder
    {
        /// <inheritdoc/>
        public override GeocoderNames GeocoderId { get => GeocoderNames.Google; }

        /// <summary>
        /// Parses test input from fake geocoders and returns a result based on that input.
        /// Used to simulate a geocoder without any lookups.
        /// </summary>
        /// <param name="geocodeRequest"></param>
        /// <returns></returns>
        public Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var requiredStatusList = ExtractResponseDetailFromAddress(geocodeRequest.Address);
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
