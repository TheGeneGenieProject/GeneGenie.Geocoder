// <copyright file="StatusTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderTests.Bing
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Models.Geo;
    using Neocoder.Services;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    /// <summary>
    /// Tests to check that all error conditions are returned as such.
    /// </summary>
    public class StatusTests
    {
        private readonly BingGeocoder bingGeocoder;

        public StatusTests()
        {
            bingGeocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();
        }

        [Fact]
        public async Task Error_is_returned_for_empty_request()
        {
            var geocodeRequest = new GeocodeRequest();

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Error_is_returned_when_bing_returns_empty_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "NULL" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Too_many_requests_is_returned_when_receiving_back_off_response_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "Header=SlowDown" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.TooManyRequests, response.ResponseStatus);
        }

        [Fact]
        public async Task Error_is_returned_when_error_details_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/ErrorDetails.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Unknown_status_codes_are_treated_as_errors_when_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/UnknownError.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Service_unavailable_is_returned_as_temporary_error()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/TemporaryError.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.TemporaryError, response.ResponseStatus);
        }

        [Fact]
        public async Task Zero_results_is_returned_when_zero_results_returned_from_bing()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/NoData.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.ZeroResults, response.ResponseStatus);
        }

        [Fact]
        public async Task Success_is_returned_on_valid_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            var response = await bingGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Success, response.ResponseStatus);
        }
    }
}
