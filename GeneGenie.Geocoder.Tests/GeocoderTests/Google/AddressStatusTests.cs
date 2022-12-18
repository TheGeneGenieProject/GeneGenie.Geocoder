// <copyright file="AddressStatusTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    /// <summary>
    /// Tests to check that all error conditions are returned as such.
    /// </summary>
    public class AddressStatusTests
    {
        private readonly GoogleGeocoder googleGeocoder;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public AddressStatusTests()
        {
            googleGeocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        /// <summary>
        /// Tests that an empty request returns an invalid request status code.
        /// </summary>
        [Fact]
        public async Task Invalid_request_is_returned_for_empty_request()
        {
            var geocodeRequest = new GeocodeRequest();

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.InvalidRequest, response.ResponseDetail.GeocodeStatus);
        }

        /// <summary>
        /// Tests that an empty response from Google is treated as an error.
        /// </summary>
        [Fact]
        public async Task Error_is_returned_when_google_returns_empty_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "NULL" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.PermanentError, response.ResponseDetail.GeocodeStatus);
        }

        /// <summary>
        /// Tests that we receive an error when Google has a permanent error.
        /// </summary>
        [Fact]
        public async Task Error_is_returned_when_receiving_junk_status_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/JunkStatus.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.PermanentError, response.ResponseDetail.GeocodeStatus);
        }

        /// <summary>
        /// Tests that the Google geocoder passes back the temporary error to the caller.
        /// </summary>
        [Fact]
        public async Task Temporary_error_is_returned_when_receiving_temporary_error_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/OverQueryLimit.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.TooManyRequests, response.ResponseDetail.GeocodeStatus);
        }

        /// <summary>
        /// Tests that the Google geocoder passes back success to the caller when the geocoding works.
        /// </summary>
        [Fact]
        public async Task Success_is_returned_on_valid_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Success, response.ResponseDetail.GeocodeStatus);
        }
    }
}
