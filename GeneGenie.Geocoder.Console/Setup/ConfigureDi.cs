// <copyright file="ConfigureDi.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console.Setup
{
    using System;
    using GeneGenie.Geocoder.ExtensionMethods;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class ConfigureDi
    {
        public static IServiceProvider BuildDi(IConfigurationRoot configuration)
        {
            var appSettings = configuration.GetSection("App").Get<AppSettings>();

            return new ServiceCollection()
                .AddSingleton(appSettings.GeocoderSettings)
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddConsole())
                .AddGeocoders()
                .BuildServiceProvider();
        }
    }
}
