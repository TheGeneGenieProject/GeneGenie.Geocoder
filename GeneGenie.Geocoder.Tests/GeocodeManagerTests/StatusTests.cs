﻿// <copyright file="StatusTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderManagerTests
{
    /// <summary>
    /// Tests for ensuring that the geocoder manager reports the correct status of a lookup depending on
    /// how many lookups failed or succeeded.
    /// </summary>
    public class StatusTests
    {
        private readonly IGeocodeManager geocodeManager;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public StatusTests()
        {
            geocodeManager = ConfigureDi.Services.GetRequiredService<IGeocodeManager>();
        }

        /// <summary>
        /// Checks that failing over from Google to Bing is possible when Google returns 0 results.
        /// </summary>
        [Fact]
        public async Task Status_is_geocoded_when_google_returns_zero_but_bing_returns_data()
        {
            var address = $"GOOGLE={GeocodeStatus.ZeroResults};BING={GeocodeStatus.Success}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.Geocoded, result.Status);
        }

        /// <summary>
        /// Checks that failing over from Bing to Google is possible when Bing returns 0 results.
        /// </summary>
        [Fact]
        public async Task Status_is_geocoded_when_bing_returns_zero_but_google_returns_data()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.Success}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.Geocoded, result.Status);
        }

        /// <summary>
        /// Checks that 'zero results' is passed back if all geocoders return 'zero results'.
        /// </summary>
        [Fact]
        public async Task Status_is_zero_results_when_all_geocoders_return_zero_results()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.ZeroResults}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.ZeroResults, result.Status);
        }

        /// <summary>
        /// Checks if all geocoders return a permanent error, that error is passed back to the caller.
        /// </summary>
        [Fact]
        public async Task Status_is_permanent_error_when_all_geocoders_return_permanent_error()
        {
            var address = $"BING={GeocodeStatus.Error};GOOGLE={GeocodeStatus.Error}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.PermanentGeocodeError, result.Status);
        }

        /// <summary>
        /// Checks that a mix of errors returns a temporary error so the caller can try again later.
        /// </summary>
        [Fact]
        public async Task Status_is_temporary_error_with_mix_of_permanent_and_temporary_errors()
        {
            var address = $"BING={GeocodeStatus.TooManyRequests};GOOGLE={GeocodeStatus.Error}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.TemporaryGeocodeError, result.Status);
        }

        /// <summary>
        /// Checks that we return 'multiple issues' if the geocoders have mixed errors.
        /// </summary>
        [Fact]
        public async Task Status_is_multiple_issues_with_mix_of_permanent_error_and_zero_results()
        {
            var address = $"BING={GeocodeStatus.ZeroResults};GOOGLE={GeocodeStatus.Error}";

            var result = await geocodeManager.GeocodeAddressAsync(address);

            Assert.Equal(AddressLookupStatus.MultipleIssues, result.Status);
        }
    }
}
