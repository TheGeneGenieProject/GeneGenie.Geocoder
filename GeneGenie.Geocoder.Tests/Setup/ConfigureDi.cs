// <copyright file="ConfigureDi.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Setup
{
    /// <summary>
    /// Set up dependencies for unit tests.
    /// </summary>
    public static class ConfigureDi
    {
        private static IServiceProvider services;

        /// <summary>
        /// Builds services for test on first access.
        /// </summary>
        public static IServiceProvider Services
        {
            get => services ??= BuildDi();
        }

        /// <summary>
        /// Builds the unit test dependencies.
        /// </summary>
        /// <param name="systemTimeIsSingleton">Pass as true to use a unit testable system time provider.</param>
        /// <returns>A service provider for resolving dependencies.</returns>
        public static IServiceProvider BuildDi(bool systemTimeIsSingleton = false)
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Bing },
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };

            var sc = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                })
                .AddGeocoders(geocoderSettings);

            // Remove the real geocoder interface registrations and replace with our fakes.
            sc.RemoveAll<IGeocoder>()
                .AddTransient<IGeocoder, FakeBingGeocoder>()
                .AddTransient<IGeocoder, FakeGoogleGeocoder>();

            // Replace the HTTP client so that the geocoders don't call out to the web and go through our test fakes instead.
            sc.RemoveAll<IGeocoderHttpClient>()
                .AddTransient<IGeocoderHttpClient, FakeGeocoderHttpClient>();

            if (systemTimeIsSingleton)
            {
                sc.AddSingleton<ITimeProvider, FakeTimeProvider>();
            }
            else
            {
                sc.AddTransient<ITimeProvider, FakeTimeProvider>();
            }

            return sc.BuildServiceProvider();
        }
    }
}
