// <copyright file="StatusParsingTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    using GeneGenie.Geocoder.Dto.Google;

    /// <summary>
    /// Tests for to ensure response parsing from Google handles all of the known status.
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
        /// Junk whitespace test data for validating the Google status code parser.
        /// </summary>
        public static IEnumerable<object[]> GoogleStatusWhitespaceData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { string.Empty },
                new object[] { " " },
                new object[] { "  " },
                new object[] { "\t" },
            };

        /// <summary>
        /// Test data for checking that we can still parse the response codes if they have extraneous spaces.
        /// </summary>
        public static IEnumerable<object[]> SpacePaddedData =>
            new List<object[]>
            {
                new object[] { " OK ", GeocodeStatus.Success },
                new object[] { " ZERO_RESULTS ", GeocodeStatus.ZeroResults },
            };

        /// <summary>
        /// Test data for checking all possible Google status code responses can be parsed.
        /// </summary>
        public static IEnumerable<object[]> CorrectResponseData =>
            new List<object[]>
            {
                new object[] { "OK", GeocodeStatus.Success },
                new object[] { "ZERO_RESULTS", GeocodeStatus.ZeroResults },
                new object[] { "xxxxx", GeocodeStatus.Error },
                new object[] { "INVALID_REQUEST", GeocodeStatus.InvalidRequest },
                new object[] { "OVER_QUERY_LIMIT", GeocodeStatus.TooManyRequests },
                new object[] { "REQUEST_DENIED", GeocodeStatus.RequestDenied },
                new object[] { "ZERO_RESULTS", GeocodeStatus.ZeroResults },
            };

        /// <summary>
        /// Tests that whitespace is logged as an error when received as a status code.
        /// </summary>
        /// <param name="source">One of the whitespace test values.</param>
        [Theory]
        [MemberData(nameof(GoogleStatusWhitespaceData))]
        public void Whitespace_and_non_data_values_are_treated_as_errors(string source)
        {
            var response = geocoder.ExtractStatus(new RootResponse { Status = source });

            Assert.Equal(GeocodeStatus.Error, response.Status);
        }

        /// <summary>
        /// Checks that spaces around the status response do not break the parsing.
        /// </summary>
        /// <param name="source">The status code with spaces.</param>
        /// <param name="expected">The expected status after parsing.</param>
        [Theory]
        [MemberData(nameof(SpacePaddedData))]
        public void Status_code_are_translated_whilst_ignoring_leading_and_trailing_spaces(string source, GeocodeStatus expected)
        {
            var response = geocoder.ExtractStatus(new RootResponse { Status = source });

            Assert.Equal(expected, response.Status);
        }

        /// <summary>
        /// Checks that all possible Google status codes can be parsed.
        /// </summary>
        /// <param name="source">The status code text received from Google.</param>
        /// <param name="expected">The expected status code after parsing.</param>
        [Theory]
        [MemberData(nameof(CorrectResponseData))]
        public void Expected_google_status_codes_can_be_parsed(string source, GeocodeStatus expected)
        {
            var response = geocoder.ExtractStatus(new RootResponse { Status = source });

            Assert.Equal(expected, response.Status);
        }
    }
}
