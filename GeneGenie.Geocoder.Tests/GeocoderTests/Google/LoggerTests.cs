// <copyright file="LoggerTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.ExtensionMethods;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models;
    using GeneGenie.Geocoder.Models.Geo;
    using GeneGenie.Geocoder.Services;
    using GeneGenie.Geocoder.Tests.Fakes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Xunit;

    /// <summary>
    /// Tests to check that all log points are called.
    /// </summary>
    public class LoggerTests
    {
        private readonly GoogleGeocoder googleGeocoder;
        private readonly FakeLogger logger;

        public LoggerTests()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };
            var serviceProvider = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .RemoveAll<ILogger<GoogleGeocoder>>()
                .AddScoped<ILogger<GoogleGeocoder>, FakeLogger>()
                .RemoveAll<IGeocoderHttpClient>()
                .AddTransient<IGeocoderHttpClient, FakeGeocoderHttpClient>()
                .BuildServiceProvider();

            googleGeocoder = serviceProvider.GetRequiredService<GoogleGeocoder>();
            logger = serviceProvider.GetRequiredService<ILogger<GoogleGeocoder>>() as FakeLogger;
        }

        [Fact]
        public async Task Trace_output_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "Anywhere" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderResponse);
        }

        [Fact]
        public async Task Empty_result_from_google_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Empty.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderReturnedNull);
        }

        [Fact]
        public async Task Missing_bounds_is_not_logged_when_viewport_exists()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingBounds.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.DoesNotContain(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        [Fact]
        public async Task Missing_bounds_is_not_logged_when_bounds_exist_and_viewport_does_not()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/BoundsExist.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.DoesNotContain(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        [Fact]
        public async Task Valid_response_has_no_critical_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.CriticalCount);
        }

        [Fact]
        public async Task Valid_response_has_no_error_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.ErrorCount);
        }

        [Fact]
        public async Task Valid_response_has_no_warning_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.WarningCount);
        }

        public static IEnumerable<object[]> ExpectedStatusResponseData =>
            new List<object[]>
            {
                new object[] { $"HttpStatusCode={nameof(HttpStatusCode.ServiceUnavailable)}", LogEventIds.GeocoderError },
                new object[] { $"HttpStatusCode={nameof(HttpStatusCode.InternalServerError)}", LogEventIds.GeocoderError },
                new object[] { $"HttpStatusCode={nameof(HttpStatusCode.SeeOther)}", LogEventIds.GeocoderError },
                new object[] { "File=Google/TemporaryError.json", LogEventIds.GeocoderError },
                new object[] { "File=Google/PermanentError.json", LogEventIds.GeocoderError },
                new object[] { "File=Google/MissingLocation.json", LogEventIds.GeocoderMissingLocation },
                new object[] { "File=Google/MissingBoundsAndViewport.json", LogEventIds.GeocoderMissingBounds },
                new object[] { "File=Google/MissingGeometry.json", LogEventIds.GeocoderMissingGeometry },
                new object[] { null, LogEventIds.GeocodeException },
            };

        [Theory]
        [MemberData(nameof(ExpectedStatusResponseData))]
        public async Task Geocoder_status_is_logged_from_google(string address, LogEventIds expected)
        {
            var geocodeRequest = new GeocodeRequest { Address = address };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)expected);
        }
    }
}
