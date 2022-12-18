// <copyright file="AppSettings.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console
{
    /// <summary>
    /// Settings for the sample app, loaded from appsettings.json, appsettings.development.json
    /// or environment variables.
    /// </summary>
    internal sealed class AppSettings
    {
        /// <summary>
        /// A list of enabled geocoders and their API keys for the demo.
        /// </summary>
        public List<GeocoderSettings> GeocoderSettings { get; set; }
    }
}
