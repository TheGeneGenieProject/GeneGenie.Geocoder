// <copyright file="GeocoderStatusMapping.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// Maps a status code returned from a geocoder service such as Google or Bing so that it can be parsed and
    /// understood in a standard way in our code.
    /// </summary>
    public class GeocoderStatusMapping
    {
        /// <summary>
        /// The API call was broken in such a way that it's not worth repeating. Possibly a missing API key.
        /// </summary>
        public bool IsPermanentError { get; set; }

        /// <summary>
        /// The geocoders returned a temporary error and you should try again later.
        /// </summary>
        public bool IsTemporaryError { get; set; }

        /// <summary>
        /// The status code for the geocode request such as 'over API limit' or 'OK'.
        /// </summary>
        public GeocodeStatus Status { get; set; }

        /// <summary>
        /// The status text returned from the geocoder API, useful for troubleshooting.
        /// </summary>
        public string StatusText { get; set; }
    }
}
