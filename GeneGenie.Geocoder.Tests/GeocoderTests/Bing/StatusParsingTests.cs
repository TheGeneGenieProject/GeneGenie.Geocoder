// <copyright file="StatusParsingTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Bing
{
    /// <summary>
    /// Tests to ensure response parsing from Bing handles all of the known status codes.
    /// </summary>

    public class StatusParsingTests
    {
        private readonly BingGeocoder geocoder;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public StatusParsingTests()
        {
            geocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();
        }

        /// <summary>
        /// Checks that the varying responses from Bing return the status codes we expect.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="expectedGeocodeStatus"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("File=Bing/Valid.json", GeocodeStatus.Success)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.BadRequest)}", GeocodeStatus.InvalidRequest)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.Forbidden)}", GeocodeStatus.RequestDenied)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.InternalServerError)}", GeocodeStatus.Error)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.SeeOther)}", GeocodeStatus.Error)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.ServiceUnavailable)}", GeocodeStatus.TemporaryError)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.TooManyRequests)}", GeocodeStatus.TooManyRequests)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.Unauthorized)}", GeocodeStatus.RequestDenied)]
        [InlineData("File=Bing/JunkStatus.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Bing/MissingLocation.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Bing/MissingBounds.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Bing/MissingGeometry.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Bing/ZeroResults.json", GeocodeStatus.ZeroResults)]
        public async Task Status_codes_are_as_expected_based_on_bing_response(string address, GeocodeStatus expectedGeocodeStatus)
        {
            var geocodeRequest = new GeocodeRequest { Address = address };

            var response = await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(expectedGeocodeStatus, response.ResponseDetail.GeocodeStatus);
        }
    }
}
