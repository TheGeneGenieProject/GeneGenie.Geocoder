// <copyright file="LoggerTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Bing
{
    /// <summary>
    /// Tests to check that all log points are called.
    /// </summary>
    public class LoggerTests
    {
        private readonly BingGeocoder geocoder;
        private readonly FakeLogger<BingGeocoder> logger;

        /// <summary>
        /// Tests for ensuring that log points in the code are hit correctly.
        /// </summary>
        public LoggerTests()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Bing },
            };
            var serviceProvider = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .RemoveAll<ILogger<BingGeocoder>>()
                .AddScoped<ILogger<BingGeocoder>, FakeLogger<BingGeocoder>>()
                .RemoveAll<IGeocoderHttpClient>()
                .AddTransient<IGeocoderHttpClient, FakeGeocoderHttpClient>()
                .BuildServiceProvider();

            geocoder = serviceProvider.GetRequiredService<BingGeocoder>();
            logger = serviceProvider.GetRequiredService<ILogger<BingGeocoder>>() as FakeLogger<BingGeocoder>;
        }

        /// <summary>
        /// Checks that the trace level output for the geocoder response is logged.
        /// </summary>
        [Fact]
        public async Task Trace_output_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "Anywhere" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderResponse);
        }

        /// <summary>
        /// Checks that if a weird null response is received, we log it.
        /// </summary>
        [Fact]
        public async Task Empty_result_from_bing_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Empty.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderReturnedNull);
        }

        /// <summary>
        /// Checks that we don't inadvertently log missing bounds if it exists in the result.
        /// </summary>
        [Fact]
        public async Task Missing_bounds_is_not_logged_when_bounds_exist()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.DoesNotContain(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        /// <summary>
        /// Checks that critical errors are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_critical_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.CriticalCount);
        }

        /// <summary>
        /// Checks that errors are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_error_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.ErrorCount);
        }

        /// <summary>
        /// Checks that warnings are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_warning_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.WarningCount);
        }

        /// <summary>
        /// Checks that trace logs are logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_trace_log_message()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Bing/Valid.json" };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.Success);
        }

        /// <summary>
        /// Passes varying data through the geocoder to test that event ids are logged.
        /// </summary>
        /// <param name="address">Instruction that the fake HTTP client uses to emulate a response.</param>
        /// <param name="expected">The expected event id that should be logged.</param>
        [Theory]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.ServiceUnavailable)}", LogEventIds.GeocoderError)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.InternalServerError)}", LogEventIds.GeocoderError)]
        [InlineData($"HttpStatusCode={nameof(HttpStatusCode.SeeOther)}", LogEventIds.GeocoderError)]
        [InlineData("Exception", LogEventIds.GeocodeException)]
        [InlineData("File=Bing/TemporaryError.json", LogEventIds.GeocoderTemporaryError)]
        [InlineData("File=Bing/JunkStatus.json", LogEventIds.GeocoderUnknownContentStatus)]
        [InlineData("File=Bing/MissingLocation.json", LogEventIds.GeocoderMissingLocation)]
        [InlineData("File=Bing/MissingBounds.json", LogEventIds.GeocoderMissingBounds)]
        [InlineData("File=Bing/MissingGeometry.json", LogEventIds.GeocoderMissingGeometry)]
        [InlineData("File=Bing/NullResults.json", LogEventIds.GeocoderMissingResults)]
        [InlineData("File=Bing/ZeroResults.json", LogEventIds.GeocoderZeroResults)]
        [InlineData(null, LogEventIds.GeocoderInputEmpty)]
        public async Task Geocoder_status_is_logged_from_bing(string address, LogEventIds expected)
        {
            var geocodeRequest = new GeocodeRequest { Address = address };

            await geocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)expected);
        }
    }
}
