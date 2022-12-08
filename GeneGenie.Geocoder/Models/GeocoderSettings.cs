// <copyright file="GeocoderSettings.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models
{
    /// <summary>
    /// Holds the API key for each geocoder.
    /// </summary>
    public class GeocoderSettings
    {
        /// <summary>
        /// The key used for accessing the API of the geocoder service.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The unique geocoder id.
        /// </summary>
        public GeocoderNames GeocoderName { get; set; }
    }
}
