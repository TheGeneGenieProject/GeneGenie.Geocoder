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

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for geocoding addresses and managing the scheduling of geocode requests.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add the registrations to.</param>
        /// <returns>The service collection with all geocoder classes registered.</returns>
        public static IServiceCollection AddGeocoders(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<KeyComposer>()
                .AddTransient(sp => ResolveBing(sp))
                .AddTransient(sp => ResolveGoogle(sp))
                .AddTransient<IGeocoder, BingGeocoder>(sp => ResolveBing(sp))
                .AddTransient<IGeocoder, GoogleGeocoder>(sp => ResolveGoogle(sp))
                .AddTransient<IGeocoderSelector, InMemoryGeocoderSelector>()
                .AddTransient<InMemoryGeocoderSelector>()
                .AddTransient<ITimeProvider, TimeProvider>()
                .AddTransient<IGeocoderHttpClient, GeocoderHttpClient>()
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
