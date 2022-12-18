// <copyright file="InMemoryGeocoderSelector.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services.Selectors
{
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
        private readonly List<GeocoderNames> emptyList = new();
        private readonly List<GeocoderSettings> geocoderSettings;
        private readonly IServiceProvider serviceProvider;
        private readonly ITimeProvider timeProvider;
        private List<GeocoderState> currentGeocoderStates;

        /// <summary>
        /// Creates an instance of the geocoder manager to track multiple geocoding attempts across different geocoders.
        /// </summary>
        /// <param name="geocoderSettings">A list of geocoder settings, one for each active geocoder.</param>
        /// <param name="serviceProvider">A service provider so we can retrieve the registered geocoders without requiring a hard reference.</param>
        /// <param name="timeProvider">
        /// A class the tells us the current system time in order to handle throttling of API requests.
        /// Handled this way so that we can unit test this class with specific times.
        /// </param>
        public InMemoryGeocoderSelector(List<GeocoderSettings> geocoderSettings, IServiceProvider serviceProvider, ITimeProvider timeProvider)
        {
            this.geocoderSettings = geocoderSettings;
            this.serviceProvider = serviceProvider;
            this.timeProvider = timeProvider;
            ResetState();
        }

        /// <summary>
        /// Select the next geocoder from a list based on previous usage.
        /// </summary>
        /// <returns>An instance of a geocoder that can be used to geocode an address.</returns>
        public async Task<IGeocoder> SelectNextGeocoderAsync()
        {
            return await SelectNextGeocoderAsync(emptyList);
        }

        /// <summary>
        /// Select the next geocoder from a list based on previous usage, excluding the passed geocoders which may have been throttled.
        /// </summary>
        /// <param name="excludeGeocoders">A list of geocoder ids to avoid calling for now.</param>
        /// <returns>An instance of a geocoder that can be used to geocode an address.</returns>
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

        private void ResetState()
        {
            var yesterday = timeProvider.UtcNow().AddDays(-1);
            currentGeocoderStates = geocoderSettings
                .Select(gs => new GeocoderState
                {
                    DoNotUseBefore = yesterday,
                    GeocoderId = gs.GeocoderName
                })
                .ToList();
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
