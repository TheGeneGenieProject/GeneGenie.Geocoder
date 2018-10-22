// <copyright file="Program.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Console.Setup;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        private static IServiceProvider serviceProvider;
        private static ILogger<Program> logger;

        public static async Task Main(string[] args)
        {
            var configuration = ConfigureSettings.Build(args);
            serviceProvider = ConfigureDi.BuildDi(configuration);
            logger = serviceProvider.GetService<ILogger<Program>>();

            try
            {
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
                    var geocoded = await geocodeManager.GeocodeAddressAsync(address);

                    using (logger.BeginScope("Geocoding results for '{address}' via {engine}", address, geocoded.GeocoderId))
                    {
                        foreach (var location in geocoded.Locations)
                        {
                            logger.LogInformation("Result '{address}', {lat},{lng} from {source}", location.FormattedAddress, location.Location.Latitude, location.Location.Longitude, geocoded.GeocoderId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Error running console");
            }

            logger.LogInformation("Complete, press a key to quit");
            Console.ReadKey();
        }
    }
}
