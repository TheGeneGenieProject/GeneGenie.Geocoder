// <copyright file="GeocodeRequest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Models.Geo
{
    public class GeocodeRequest
    {
        public string Address { get; set; }

        public string AddressKey { get; set; }

        /// <summary>
        /// A hint passed in from the user that defines the geographical area where the search should apply additional emphasis to.
        /// For example, to focus on a part of the world or a specific part of a country and exclude similar place names found overseas.
        /// </summary>
        public Bounds BoundsHint { get; set; }

        /// <summary>If not empty then used to provide a hint to the geocoder so that the results may be localised.</summary>
        public string Language { get; set; }

        /// <summary>If not empty then used to provide a hint to the geocoder so that the results may be localised.</summary>
        public string Locale { get; set; }

        /// <summary>If not empty then used to provide a hint to the geocoder so that the results may be localised.</summary>
        public string Region { get; internal set; }
    }
}
