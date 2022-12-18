// <copyright file="ConfigureDi.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console.Setup
{
    /// <summary>
    /// Set up dependency injection for this sample app.
    /// </summary>
    public static class ConfigureDi
    {
        /// <summary>
        /// Register the geocoders based on the settings loaded from appsetttings.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceProvider BuildDi(IConfigurationRoot configuration)
        {
            var appSettings = configuration.GetSection("App").Get<AppSettings>();

            return new ServiceCollection()
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddConsole())
                .AddGeocoders(appSettings.GeocoderSettings)
                .BuildServiceProvider();
        }
    }
}
