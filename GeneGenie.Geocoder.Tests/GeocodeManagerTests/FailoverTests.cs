﻿// <copyright file="FailoverTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderManagerTests
{
    /// <summary>
    /// Tests for ensuring that the geocoders fail over to each other if they can't look up an address.
    /// </summary>
    public class FailoverTests
    {
        private readonly IGeocodeManager geocodeManager;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public FailoverTests()
        {
            geocodeManager = ConfigureDi.Services.GetRequiredService<IGeocodeManager>();
        }

        /// <summary>
        /// Checks that Google is used when Bing returns 'no results'.
        /// </summary>
        [Fact]
        public async Task Bing_is_repeatedly_used_when_google_fails_to_return_a_result()
        {
            var address = $"GOOGLE={GeocodeStatus.ZeroResults};BING={GeocodeStatus.Success}";

            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
        }

        /// <summary>
        /// Checks that Bing is used when Google returns 'no results'.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Google_is_repeatedly_used_when_bing_fails_to_return_a_result()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.Success}";

            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocodeManager.GeocodeAddressAsync(address)).GeocoderId);
        }

        /// <summary>
        /// Checks that none of the geocoders is marked as returning data when they all return nothing.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Address_is_not_geocoded_when_all_geocoders_fail_to_return_a_result()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.ZeroResults}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(GeocoderNames.NotSet, result.GeocoderId);
        }
    }
}
