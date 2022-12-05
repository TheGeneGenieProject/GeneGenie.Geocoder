// <copyright file="RoundRobinTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderSelectorTests
{
    /// <summary>
    /// Tests for ensuring the Round Robin scheduler for the geocoders rotates the services.
    /// </summary>
    public class RoundRobinTests
    {
        private readonly IGeocoderSelector geocoderSelector;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public RoundRobinTests()
        {
            geocoderSelector = ConfigureDi.Services.GetRequiredService<InMemoryGeocoderSelector>();
        }

        /// <summary>
        /// Checks that Bing can be resolved first as it is registered first.
        /// </summary>
        [Fact]
        public async Task Bing_is_resolved_first()
        {
            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Equal(GeocoderNames.Bing, geocoder.GeocoderId);
        }

        /// <summary>
        /// Checks that Google can be resolved second as it is registered after Bing.
        /// </summary>
        [Fact]
        public async Task Google_is_resolved_second()
        {
            await geocoderSelector.SelectNextGeocoderAsync();

            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Equal(GeocoderNames.Google, geocoder.GeocoderId);
        }

        /// <summary>
        /// Checks that Bing can be resolved on the third pass as we loop back to the start.
        /// </summary>
        [Fact]
        public async Task Bing_is_resolved_again_after_google()
        {
            await geocoderSelector.SelectNextGeocoderAsync();
            await geocoderSelector.SelectNextGeocoderAsync();

            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Equal(GeocoderNames.Bing, geocoder.GeocoderId);
        }
    }
}
