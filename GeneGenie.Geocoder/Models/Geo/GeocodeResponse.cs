// <copyright file="GeocodeResponse.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    public class GeocodeResponse
    {
        public List<GeocodeResponseLocation> Locations { get; set; } = new List<GeocodeResponseLocation>();

        public GeocoderNames GeocoderId { get; set; }

        public AddressLookupStatus Status { get; set; }
    }
}
