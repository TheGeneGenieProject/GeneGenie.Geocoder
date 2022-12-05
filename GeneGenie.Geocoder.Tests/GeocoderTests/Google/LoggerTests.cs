// <copyright file="LoggerTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    /// <summary>
    /// Tests to check that all log points are called.
    /// </summary>
    public class LoggerTests
    {
        private readonly GoogleGeocoder googleGeocoder;
        private readonly FakeLogger<GoogleGeocoder> logger;

        /// <summary>
        /// Tests for ensuring that log points in the code are hit correctly.
        /// </summary>
        public LoggerTests()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };
            var serviceProvider = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .RemoveAll<ILogger<GoogleGeocoder>>()
                .AddScoped<ILogger<GoogleGeocoder>, FakeLogger<GoogleGeocoder>>()
                .RemoveAll<IGeocoderHttpClient>()
                .AddTransient<IGeocoderHttpClient, FakeGeocoderHttpClient>()
                .BuildServiceProvider();

            googleGeocoder = serviceProvider.GetRequiredService<GoogleGeocoder>();
            logger = serviceProvider.GetRequiredService<ILogger<GoogleGeocoder>>() as FakeLogger<GoogleGeocoder>;
        }

        /// <summary>
        /// Unit test data for faking HTTP responses when the geocoder is called.
        /// </summary>
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
                new object[] { "File=Google/ZeroResults.json", LogEventIds.GeocoderZeroResults },
                new object[] { null, LogEventIds.GeocoderInputEmpty },
            };

        /// <summary>
        /// Checks that the trace level output for the geocoder response is logged.
        /// </summary>
        [Fact]
        public async Task Trace_output_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "Anywhere" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderResponse);
        }

        /// <summary>
        /// Checks that if a weird null response is received, we log it.
        /// </summary>
        [Fact]
        public async Task Empty_result_from_google_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Empty.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderReturnedNull);
        }

        /// <summary>
        /// Checks that a missing bounds message is not logged if a viewport is received in the response.
        /// </summary>
        [Fact]
        public async Task Missing_bounds_is_not_logged_when_viewport_exists()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingBounds.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.DoesNotContain(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        /// <summary>
        /// Checks that we don't inadvertently log missing bounds if it exists in the result.
        /// </summary>
        [Fact]
        public async Task Missing_bounds_is_not_logged_when_bounds_exist_and_viewport_does_not()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/BoundsExist.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.DoesNotContain(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        /// <summary>
        /// Checks that critical errors are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_critical_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.CriticalCount);
        }

        /// <summary>
        /// Checks that errors are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_error_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.ErrorCount);
        }

        /// <summary>
        /// Checks that warnings are not logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_no_warning_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.WarningCount);
        }

        /// <summary>
        /// Checks that trace logs are logged for a successful operation.
        /// </summary>
        [Fact]
        public async Task Valid_response_has_trace_log_message()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.Success);
        }

        /// <summary>
        /// Passes varying data through the geocoder to test that event ids are logged.
        /// </summary>
        /// <param name="address">Instruction that the fake HTTP client uses to emulate a response.</param>
        /// <param name="expected">The expected event id that should be logged.</param>
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
