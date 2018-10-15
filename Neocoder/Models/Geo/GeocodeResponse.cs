// <copyright file="GeocodeResponse.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Models.Geo
{
    using System.Collections.Generic;
    using Neocoder.Services;

    public class GeocodeResponse
    {
        public List<GeocodeResponseLocationV2> Locations { get; set; } = new List<GeocodeResponseLocationV2>();

        public GeocoderNames GeocoderId { get; set; }

        public AddressLookupStatus Status { get; set; }
    }
}
