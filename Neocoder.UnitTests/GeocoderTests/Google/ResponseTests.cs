// <copyright file="ResponseTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderTests.Google
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Models.Geo;
    using Neocoder.Services;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    /// <summary>
    /// Tests to check how valid responses are parsed.
    /// </summary>
    public class ResponseTests
    {
        private readonly GoogleGeocoder googleGeocoder;

        public ResponseTests()
        {
            googleGeocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        [Fact]
        public async Task Bounds_are_preferred_over_viewport_when_both_are_present()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/HasBoundsAndViewport.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);
            var bounds = response.Locations.Single().Bounds;

            Assert.Equal(1.1, bounds.NorthEast.Latitude);
        }

        [Fact]
        public async Task Viewport_is_used_when_bounds_are_null()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingBounds.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);
            var bounds = response.Locations.Single().Bounds;

            Assert.Equal(5.5, bounds.NorthEast.Latitude);
        }

        /// <summary>
        /// Test to ensure that the JSON returned from a geocoder can be mapped to properties, even if they differ in their casing.
        /// </summary>
        [Fact]
        public async Task Case_of_results_is_ignored_by_serialiser()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MixedCase.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);
            var bounds = response.Locations.Single().Bounds;

            Assert.Equal(5.5, bounds.NorthEast.Latitude);
        }
    }
}
