// <copyright file="GeocoderState.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models
{
    /// <summary>
    /// Used to track usage of a geocoder when called through <see cref="GeocodeManager"/> so that
    /// the load can be spread out amongst multiple geocoders.
    /// </summary>
    public class GeocoderState
    {
        /// <summary>
        /// When we receive a 'back-off' response from a geocoder, this property is used to delay the next API call
        /// so we don't get banned.
        /// </summary>
        public DateTimeOffset DoNotUseBefore { get; set; }

        /// <summary>
        /// The unique geocoder ID.
        /// </summary>
        public GeocoderNames GeocoderId { get; set; }

        /// <summary>
        /// The number of times we've called this geocoder.
        /// </summary>
        public int RequestCount { get; set; }
    }
}
