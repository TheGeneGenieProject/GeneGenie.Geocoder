// <copyright file="FailoverTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderManagerTests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Services;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    /// <summary>
    /// Tests for ensuring that the geocoders fail over to each other if they can't look up an address.
    /// </summary>
    public class FailoverTests
    {
        private readonly GeocodeManager geocodeManager;

        public FailoverTests()
        {
            geocodeManager = ConfigureDi.Services.GetRequiredService<GeocodeManager>();
        }

        [Fact]
        public async Task Bing_is_repeatedly_used_when_google_fails_to_return_a_result()
        {
            var address = $"GOOGLE={GeocodeStatus.ZeroResults};BING={GeocodeStatus.Success}";

            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
        }

        [Fact]
        public async Task Google_is_repeatedly_used_when_bing_fails_to_return_a_result()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.Success}";

            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
        }

        [Fact]
        public async Task Address_is_not_geocoded_when_all_geocoders_fail_to_return_a_result()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.ZeroResults}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(GeocoderNames.NotSet, result.GeocoderId);
        }
    }
}
