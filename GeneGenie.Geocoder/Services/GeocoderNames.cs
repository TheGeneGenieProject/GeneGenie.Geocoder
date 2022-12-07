// <copyright file="GeocoderNames.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// All currently implemented geocoders.
    /// </summary>
    public enum GeocoderNames
    {
        /// <summary>
        /// Not a valid state, if a usage of this enum returns this status
        /// then your geocoder has an issue which needs addressing.
        /// </summary>
        NotSet,
        
        /// <summary>
        /// Geocodes against the Microsoft Bing Location API.
        /// </summary>
        Bing = 1,
        
        /// <summary>
        /// Geocodes against the Google Places API.
        /// </summary>
        Google = 2,
    }
}
