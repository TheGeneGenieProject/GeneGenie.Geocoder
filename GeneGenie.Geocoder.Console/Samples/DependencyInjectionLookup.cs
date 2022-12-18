// <copyright file="DependencyInjectionLookup.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console.Samples
{
    /// <summary>
    /// Sample of how to use the geocoder in a .Net Core environment with Dependency Injection.
    /// </summary>
    internal abstract class DependencyInjectionLookup
    {
        /// <summary>
        /// Sets up standard .Net Core configuration, DI, logging and registers the geocoder with DI.
        /// Then geocodes some real and some 'dodgy' addresses as an example of usage.
        /// </summary>
        /// <param name="args">Command line arguments that are passed to the .Net runtime from which the environment (Build, Release etc.)
        /// can be picked up and used to select an alternative configuration file, as per usual .Net Core configuration.
        /// </param>
        /// <returns>A <see cref="Task"/> that should be awaited.</returns>
        internal static async Task ExecuteAsync(string[] args)
        {
            var configuration = ConfigureSettings.Build(args);
            var serviceProvider = ConfigureDi.BuildDi(configuration);
            var logger = serviceProvider.GetService<ILogger>();

            var addresses = new List<string>
            {
                "Luton",
                "Unknown",
                "?",
                "75 Beadlow Road, Lewsey Farm, Luton, LU4 0QZ",
                "75 Beadlow Road, Lewsey Farm, Luton, LU4 0QZ, UK",
            };

            var geocodeManager = serviceProvider.GetRequiredService<GeocodeManager>();
            foreach (var address in addresses)
            {
                var geocoded = await geocodeManager.GeocodeAddressAsync(address).ConfigureAwait(false);

                using (logger.BeginScope("Geocoding results for '{Address}' via {Engine}", address, geocoded.GeocoderId))
                {
                    foreach (var location in geocoded.Locations)
                    {
                        logger.LogInformation("Result '{Address}', {Lat}, {Lng} from {Source}", location.FormattedAddress, location.Location.Latitude, location.Location.Longitude, geocoded.GeocoderId);
                    }
                }
            }
        }
    }
}
