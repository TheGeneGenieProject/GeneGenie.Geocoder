// <copyright file="GoogleStatusParsingTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.Geo
{
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Services;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    public class GoogleStatusParsingTests
    {
        private readonly GoogleGeocoder geocoder;

        public GoogleStatusParsingTests()
        {
            geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        public static IEnumerable<object[]> GoogleStatusWhitespaceData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { string.Empty },
                new object[] { " " },
                new object[] { "  " },
                new object[] { "\t" },
            };

        public static IEnumerable<object[]> SpacePaddedData =>
            new List<object[]>
            {
                new object[] { " OK ", GeocodeStatus.Success },
                new object[] { " ZERO_RESULTS ", GeocodeStatus.ZeroResults },
            };

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

        [Theory]
        [MemberData(nameof(GoogleStatusWhitespaceData))]
        public void Whitespace_and_non_data_values_are_treated_as_errors(string source)
        {
            var response = geocoder.ExtractStatus(source);

            Assert.Equal(GeocodeStatus.Error, response.Status);
        }

        [Theory]
        [MemberData(nameof(SpacePaddedData))]
        public void Status_code_are_translated_whilst_ignoring_leading_and_trailing_spaces(string source, GeocodeStatus expected)
        {
            var response = geocoder.ExtractStatus(source);

            Assert.Equal(expected, response.Status);
        }

        [Theory]
        [MemberData(nameof(CorrectResponseData))]
        public void Expected_google_status_codes_can_be_parsed(string source, GeocodeStatus expected)
        {
            var response = geocoder.ExtractStatus(source);

            Assert.Equal(expected, response.Status);
        }
    }
}
