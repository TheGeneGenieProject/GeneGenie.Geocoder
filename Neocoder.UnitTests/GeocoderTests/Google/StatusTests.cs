// <copyright file="StatusTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderTests.Google
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
        private readonly GoogleGeocoder googleGeocoder;

        public StatusTests()
        {
            googleGeocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        [Fact]
        public async Task Error_is_returned_for_empty_request()
        {
            var geocodeRequest = new GeocodeRequest();

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Error_is_returned_when_google_returns_empty_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "NULL" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Error_is_returned_when_receiving_permanent_error_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/PermanentError.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Error_is_returned_when_receiving_temporary_error_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/TemporaryError.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Error, response.ResponseStatus);
        }

        [Fact]
        public async Task Success_is_returned_on_valid_response()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            var response = await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(GeocodeStatus.Success, response.ResponseStatus);
        }
    }
}
