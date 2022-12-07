// <copyright file="AddressStatusTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Bing
{
    /// <summary>
    /// Tests to check that all status codes are returned when using the Bing address lookup.
    /// </summary>
    public class AddressStatusTests
    {
        private readonly BingGeocoder bingGeocoder;

        /// <summary>
        /// Creates a new test instance. Called by xunit only.
        /// </summary>
        public AddressStatusTests()
        {
            bingGeocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();
        }

        /// <summary>
        /// Tests that when we pass the Bing geocoder an empty request it returns the correct status.
        /// </summary>
        [Fact]
        public async Task Invalid_request_is_returned_for_empty_request()
        {
            var geocodeRequest = new GeocodeRequest();

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.InvalidRequest, response.ResponseStatus);
        }

        /// <summary>
        /// Tests that the Bing geocoder returns the correct status for no results.
        /// </summary>
        [Fact]
        public async Task Error_is_returned_when_bing_returns_empty_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "NULL" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        /// <summary>
        /// Checks that we can parse the Bing back-off response.
        /// </summary>
        [Fact]
        public async Task Too_many_requests_is_returned_when_receiving_back_off_response_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "Header=SlowDown" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.TooManyRequests, response.ResponseStatus);
        }

        /// <summary>
        /// Checks that error responses are parsed from Bing and returned to the caller.
        /// </summary>
        [Fact]
        public async Task Error_is_returned_when_error_details_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/ErrorDetails.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        /// <summary>
        /// Checks that new status codes are logged as errors.
        /// </summary>
        [Fact]
        public async Task Unknown_status_codes_are_treated_as_errors_when_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/UnknownError.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        /// <summary>
        /// Checks that Bing being offline for a while returns a temporary error rather than permanent.
        /// </summary>
        [Fact]
        public async Task Service_unavailable_is_returned_as_temporary_error()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/TemporaryError.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.TemporaryError, response.ResponseStatus);
        }

        /// <summary>
        /// Ensures 'zero results' status code is passed back to the caller.
        /// </summary>
        [Fact]
        public async Task Zero_results_is_returned_when_zero_results_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/NoData.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.ZeroResults, response.ResponseStatus);
        }

        /// <summary>
        /// Ensures 'zero results' status code is passed back to the caller.
        /// </summary>
        [Fact]
        public async Task Success_is_returned_on_valid_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Success, response.ResponseStatus);
        }
    }
}
