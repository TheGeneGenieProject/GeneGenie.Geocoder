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

        public List<GeocodeResponseLocation> Locations { get; set; } = new List<GeocodeResponseLocation>();

        public GeocoderNames GeocoderId { get; set; }

        public AddressLookupStatus Status { get; set; }
    }
}
