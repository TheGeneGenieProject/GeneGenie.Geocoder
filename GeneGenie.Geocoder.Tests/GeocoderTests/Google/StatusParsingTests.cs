// <copyright file="StatusParsingTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    /// <summary>
    /// Tests to ensure response parsing from Google handles all of the known status codes.
    /// </summary>
    public class StatusParsingTests
    {
        private readonly GoogleGeocoder geocoder;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public StatusParsingTests()
        {
            geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        /// <summary>
        /// Checks that the varying responses from Google return the status codes we expect.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="expectedGeocodeStatus"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("File=Google/Valid.json", GeocodeStatus.Success)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.ServiceUnavailable)}", GeocodeStatus.Error)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.InternalServerError)}", GeocodeStatus.Error)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.SeeOther)}", GeocodeStatus.Error)]
        [InlineData("File=Google/JunkStatus.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Google/OverQueryLimit.json", GeocodeStatus.TooManyRequests)]
        [InlineData("File=Google/MissingLocation.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Google/MissingBoundsAndViewport.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Google/MissingGeometry.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Google/RequestDenied.json", GeocodeStatus.PermanentError)]
        [InlineData("File=Google/ZeroResults.json", GeocodeStatus.ZeroResults)]
        public async Task Status_codes_are_as_expected_based_on_google_response(string address, GeocodeStatus expectedGeocodeStatus)
        {
            var geocodeRequest = new GeocodeRequest { Address = address };

            var response = await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(expectedGeocodeStatus, response.ResponseDetail.GeocodeStatus);
        }
    }
}
