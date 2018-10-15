// <copyright file="InMemoryGeocoderSelector.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Services.Selectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.Interfaces;
    using Neocoder.Models;
    using Neocoder.Services;

    /// <summary>
    /// A geocode selector (finds the next available geocoder) that stores the current state of the
    /// geocoders in-memory.
    /// Useful for testing and running geocoding from a single app.
    /// Should not be used in a multi-process setup as every process would have it's own instance
    /// and would not be aware of the selections of others, hence it could not select the correct
    /// geocoder.
    /// </summary>
    public class InMemoryGeocoderSelector : IGeocoderSelector
    {
        private readonly List<GeocoderNames> emptyList = new List<GeocoderNames>();
        private readonly IServiceProvider serviceProvider;
        private readonly ITimeProvider timeProvider;
        private List<GeocoderState> currentGeocoderStates;

        public InMemoryGeocoderSelector(IServiceProvider serviceProvider, ITimeProvider timeProvider)
        {
            this.serviceProvider = serviceProvider;
            this.timeProvider = timeProvider;
            ResetState();
        }

        public async Task<IGeocoder> SelectNextGeocoderAsync()
        {
            return await SelectNextGeocoderAsync(emptyList);
        }

        public async Task<IGeocoder> SelectNextGeocoderAsync(List<GeocoderNames> excludeGeocoders)
        {
            var nextGeocoder = RoundRobin(excludeGeocoders);

            if (nextGeocoder == null)
            {
                return await Task.FromResult<IGeocoder>(null);
            }

            nextGeocoder.RequestCount++;

            return serviceProvider
                .GetServices<IGeocoder>()
                .FirstOrDefault(g => g.GeocoderId == nextGeocoder.GeocoderId);
        }

        /// <summary>
        /// Return the status of the passed Geocoder.
        /// Used for testing geocoder status filtering in unit tests.
        /// </summary>
        /// <param name="geocoderId"></param>
        /// <returns></returns>
        public GeocoderState GeocoderStatusById(GeocoderNames geocoderId)
        {
            return currentGeocoderStates
                .FirstOrDefault(g => g.GeocoderId == geocoderId);
        }

        public void ResetState()
        {
            var yesterday = timeProvider.UtcNow().AddDays(-1);
            currentGeocoderStates = new List<GeocoderState>
            {
                new GeocoderState { DoNotUseBefore = yesterday, GeocoderId = GeocoderNames.Bing },
                new GeocoderState { DoNotUseBefore = yesterday, GeocoderId = GeocoderNames.Google },
            };
        }

        /// <summary>
        /// A simple round-robin / flip-flop geocoder selection that rotates the geocoders based on usage.
        /// </summary>
        /// <param name="excludeGeocoders"></param>
        /// <returns></returns>
        private GeocoderState RoundRobin(List<GeocoderNames> excludeGeocoders)
        {
            var nextGeocoder = currentGeocoderStates
                .Where(g => !excludeGeocoders.Contains(g.GeocoderId))
                .Where(g => g.DoNotUseBefore < timeProvider.UtcNow()) // Geocoder may have been throttled.
                .OrderBy(g => g.RequestCount) // Least used geocoder first.
                .FirstOrDefault();
            return nextGeocoder;
        }
    }
}
