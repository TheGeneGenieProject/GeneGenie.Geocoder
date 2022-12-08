// <copyright file="SimpleLookup.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console.Samples
{
    /// <summary>
    /// Sample of how to use the geocoder in .Net Core without doing the whole Dependency Injection dance (it is handled internally by the library).
    /// </summary>
    internal class SimpleLookup
    {
        /// <summary>
        /// Sample showing how to use the geocoder in as simple a manner as possible.
        /// The sample demonstrates the 'round robin' approach to geocoder selection (first Bing will be used, then Google).
        /// </summary>
        /// <returns>A <see cref="Task"/> that should be awaited.</returns>
        internal async Task ExecuteAsync()
        {
            // Define the API keys for the geocoders we'll be using.
            var geocoderSettings = new List<GeocoderSettings>
            {
                // The following is where we populate the API keys. Best practice is to put these in config files, not in source.
                // Your Bing API key goes in the next line.
                new GeocoderSettings { ApiKey = "", GeocoderName = Services.GeocoderNames.Bing },
                // Your Google API key goes in the next line.
                new GeocoderSettings { ApiKey = "", GeocoderName = Services.GeocoderNames.Google },
            };

            if (geocoderSettings.Any(gs => string.IsNullOrWhiteSpace(gs.ApiKey)))
            {
                System.Console.WriteLine("API keys are blank, you need to provide the API keys from Google and Bing.");
                return;
            }

            // Instead of doing 'var g = new GeocodeManager' we use a factory method which initialises the library.
            var geocodeManager = GeocodeManager.Create(geocoderSettings);

            // This first lookup will be handled by Bing (unless it fails to resolve the address, which will then fail over to Google).
            var firstResult = await geocodeManager.GeocodeAddressAsync("10 Downing St., London, UK");
            System.Console.WriteLine($"Result of first lookup, used {firstResult.GeocoderId}, status of {firstResult.Status} with {firstResult.Locations.Count} results.");
            foreach (var foundLocation in firstResult.Locations)
            {
                System.Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
            }

            // The following lookup then uses the other geocoder service.
            var secondResult = await geocodeManager.GeocodeAddressAsync("The Acropolis, Greece");
            System.Console.WriteLine($"Result of second lookup, used {secondResult.GeocoderId}, status of {secondResult.Status} with {secondResult.Locations.Count} results.");
            foreach (var foundLocation in secondResult.Locations)
            {
                System.Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
            }
        }
    }
}
