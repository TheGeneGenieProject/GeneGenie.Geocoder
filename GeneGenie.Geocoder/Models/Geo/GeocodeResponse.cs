// <copyright file="GeocodeResponse.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    /// <summary>
    /// A response to a geocode call that hides the implementation details of the geocoders
    /// so you don't have to worry about the different output formats.
    /// </summary>
    public class GeocodeResponse
    {
        /// <summary>
        /// If this geocode call succeeded then this will contain a list of possible locations to present to the user.
        /// </summary>
        public List<GeocodeResponseLocation> Locations { get; set; } = new List<GeocodeResponseLocation>();

        /// <summary>
        /// The unique id of the geocoder that generated this response.
        /// When you use the geocode manager you might get any one of the registered geocoders.
        /// </summary>
        public GeocoderNames GeocoderId { get; set; }

        /// <summary>
        /// Holds the status of an address lookup.
        /// </summary>
        public AddressLookupStatus Status { get; set; }
    }
}
