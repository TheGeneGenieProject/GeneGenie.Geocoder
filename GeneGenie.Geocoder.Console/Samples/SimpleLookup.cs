// <copyright file="SimpleLookup.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Models;

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
                new GeocoderSettings { ApiKey = "Your Bing API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Bing },
                new GeocoderSettings { ApiKey = "Your Google API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Google },
            };

            // Instead of doing 'var g = new GeocodeManager' we use a factory method which initialises the library.
            var geocodeManager = GeocodeManager.Create(geocoderSettings);

            // This first lookup will be handled by Bing (unless it fails to resolve the address, which will then fail over to Google).
            var firstResult = await geocodeManager.GeocodeAddressAsync("10 Downing St., London, UK");
            Console.WriteLine($"Result of first lookup, used {firstResult.GeocoderId}, status of {firstResult.Status} with {firstResult.Locations.Count} results.");
            foreach (var foundLocation in firstResult.Locations)
            {
                Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
            }

            // The following lookup then uses the other geocoder service.
            var secondResult = await geocodeManager.GeocodeAddressAsync("The Acropolis, Greece");
            Console.WriteLine($"Result of second lookup, used {secondResult.GeocoderId}, status of {secondResult.Status} with {secondResult.Locations.Count} results.");
            foreach (var foundLocation in secondResult.Locations)
            {
                Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
            }
        }
    }
}
