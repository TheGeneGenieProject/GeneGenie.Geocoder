// <copyright file="StatusParsingTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Bing
{
    using GeneGenie.Geocoder.Dto.Bing;

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
        /// Tests that whitespace is logged as an error when received as a status code.
        /// </summary>
        /// <param name="source">One of the whitespace test values.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        public void Whitespace_and_non_data_values_are_treated_as_errors(string source)
        {
            var response = geocoder.ExtractStatus(new RootResponse { StatusDescription = source });

            Assert.Equal(GeocodeStatus.Error, response.Status);
        }

        /// <summary>
        /// Checks that spaces around the status response do not break the parsing as well as casing of the status code.
        /// </summary>
        /// <param name="source">The status code text.</param>
        /// <param name="expected">The expected status after parsing.</param>
        [Theory]
        [InlineData(" OK ", GeocodeStatus.Success)]
        [InlineData(" Unauthorized ", GeocodeStatus.RequestDenied)]
        [InlineData(" Service UnAvaIlaBle ", GeocodeStatus.TemporaryError)]
        [InlineData("OK", GeocodeStatus.Success)]
        [InlineData("Unauthorized", GeocodeStatus.RequestDenied)]
        [InlineData("Service Unavailable", GeocodeStatus.TemporaryError)]
        [InlineData(" made up status code ", GeocodeStatus.Error)]
        public void Expected_bing_status_codes_can_be_parsed(string source, GeocodeStatus expected)
        {
            var response = geocoder.ExtractStatus(new RootResponse { StatusDescription = source });

            Assert.Equal(expected, response.Status);
        }
    }
}
