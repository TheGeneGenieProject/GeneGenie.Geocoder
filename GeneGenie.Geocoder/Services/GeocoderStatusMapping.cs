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
        public bool IsPermanentError { get; set; }

        public bool IsTemporaryError { get; set; }

        public GeocodeStatus Status { get; set; }

        public string StatusText { get; set; }
    }
}
