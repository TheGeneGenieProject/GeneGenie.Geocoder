// <copyright file="RoundRobinTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderSelectorTests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Interfaces;
    using Neocoder.Services;
    using Neocoder.Services.Selectors;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    public class RoundRobinTests
    {
        private readonly IGeocoderSelector geocoderSelector;

        public RoundRobinTests()
        {
            geocoderSelector = ConfigureDi.Services.GetRequiredService<InMemoryGeocoderSelector>();
        }

        [Fact]
        public async Task Bing_is_resolved_first()
        {
            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Equal(GeocoderNames.Bing, geocoder.GeocoderId);
        }

        [Fact]
        public async Task Google_is_resolved_second()
        {
            await geocoderSelector.SelectNextGeocoderAsync();

            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Equal(GeocoderNames.Google, geocoder.GeocoderId);
        }

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
