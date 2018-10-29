// <copyright file="ServiceCollectionExtensions.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models;
    using GeneGenie.Geocoder.Services;
    using GeneGenie.Geocoder.Services.Selectors;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods used for registering and resolving the services used by this library with the frameworks dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for geocoding addresses and managing the scheduling of geocode requests.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add the registrations to.</param>
        /// <param name="geocoderSettings">An optional list of API keys to use with the geocoders. If not passed in here then you need to register
        /// these settings with your DI framework yourself.</param>
        /// <returns>The service collection with all geocoder classes registered.</returns>
        public static IServiceCollection AddGeocoders(this IServiceCollection serviceCollection, List<GeocoderSettings> geocoderSettings = null)
        {
            if (geocoderSettings != null)
            {
                serviceCollection.AddSingleton(geocoderSettings);
            }

            return serviceCollection
                .AddTransient(sp => ResolveBing(sp))
                .AddTransient(sp => ResolveGoogle(sp))
                .AddTransient<IGeocoder, BingGeocoder>(sp => ResolveBing(sp))
                .AddTransient<IGeocoder, GoogleGeocoder>(sp => ResolveGoogle(sp))
                .AddTransient<IGeocoderSelector, InMemoryGeocoderSelector>()
                .AddTransient<InMemoryGeocoderSelector>()
                .AddTransient<ITimeProvider, TimeProvider>()
                .AddTransient<IGeocoderHttpClient, GeocoderHttpClient>()
                .AddTransient<IGeocodeManager, GeocodeManager>()
                .AddTransient<GeocodeManager>()
                .AddHttpClient();
        }

        private static BingGeocoder ResolveBing(IServiceProvider sp)
        {
            var settings = sp.GetRequiredService<List<GeocoderSettings>>()
                .FirstOrDefault(g => g.GeocoderName == GeocoderNames.Bing);
            var httpClient = sp.GetRequiredService<IGeocoderHttpClient>();
            var logger = sp.GetRequiredService<ILogger<BingGeocoder>>();

            return new BingGeocoder(httpClient, settings, logger);
        }

        private static GoogleGeocoder ResolveGoogle(IServiceProvider sp)
        {
            var settings = sp.GetRequiredService<List<GeocoderSettings>>()
                .FirstOrDefault(g => g.GeocoderName == GeocoderNames.Google);
            var httpClient = sp.GetRequiredService<IGeocoderHttpClient>();
            var logger = sp.GetRequiredService<ILogger<GoogleGeocoder>>();

            return new GoogleGeocoder(httpClient, settings, logger);
        }
    }
}
