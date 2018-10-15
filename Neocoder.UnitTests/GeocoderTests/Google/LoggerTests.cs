// <copyright file="LoggerTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.GeocoderTests.Google
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Neocoder.ExtensionMethods;
    using Neocoder.Interfaces;
    using Neocoder.Models;
    using Neocoder.Models.Geo;
    using Neocoder.Services;
    using Neocoder.UnitTests.Fakes;
    using Xunit;

    /// <summary>
    /// Tests to check that all log points are called.
    /// </summary>
    public class LoggerTests
    {
        private readonly GoogleGeocoder googleGeocoder;
        private readonly Fakelogger logger;

        public LoggerTests()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };
            var serviceProvider = new ServiceCollection()
                .AddSingleton(geocoderSettings)
                .AddGeocoders()
                .RemoveAll<ILogger<GoogleGeocoder>>()
                .AddScoped<ILogger<GoogleGeocoder>, Fakelogger>()
                .RemoveAll<IGeocoderHttpClient>()
                .AddTransient<IGeocoderHttpClient, FakeGeocoderHttpClient>()
                .BuildServiceProvider();

            googleGeocoder = serviceProvider.GetRequiredService<GoogleGeocoder>();
            logger = serviceProvider.GetRequiredService<ILogger<GoogleGeocoder>>() as Fakelogger;
        }

        [Fact]
        public async Task Exception_is_logged_for_null_request()
        {
            await googleGeocoder.GeocodeAddressAsync(null);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocodeException);
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
        public async Task Missing_geometry_is_logged_when_not_returned()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingGeometry.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingGeometry);
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
        public async Task Missing_bounds_is_logged_when_bounds_and_viewport_do_not_exist()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingBoundsAndViewport.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingBounds);
        }

        [Fact]
        public async Task Missing_location_is_logged()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/MissingLocation.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderMissingLocation);
        }

        [Fact]
        public async Task Valid_response_has_no_critical_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.Critical);
        }

        [Fact]
        public async Task Valid_response_has_no_error_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.Error);
        }

        [Fact]
        public async Task Valid_response_has_no_warning_log_messages()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/Valid.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Equal(0, logger.Warning);
        }

        [Fact]
        public async Task Geocoder_error_is_logged_when_receiving_permanent_error_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/PermanentError.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderError);
        }

        [Fact]
        public async Task Geocoder_error_is_logged_when_receiving_temporary_error_from_google()
        {
            var geocodeRequest = new GeocodeRequest { Address = "File=Google/TemporaryError.json" };

            await googleGeocoder.GeocodeAddressAsync(geocodeRequest);

            Assert.Contains(logger.LoggedEventIds, l => l.Id == (int)LogEventIds.GeocoderError);
        }
    }
}
