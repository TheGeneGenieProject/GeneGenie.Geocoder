// <copyright file="GeocodeRequest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models.Geo
{
    public class GeocodeRequest
    {
        /// <summary>
        /// Gets or sets the address to be geocoded. It is assumed that you have checked the quality of this data before attempting to geocode it.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a hint passed in from the user that defines the geographical area where the search should apply additional emphasis to.
        /// For example, to focus on a part of the world or a specific part of a country and exclude similar place names found overseas.
        /// </summary>
        public Bounds BoundsHint { get; set; }

        /// <summary>Gets or sets a hint to the geocoder so that the results may be localised.
        /// Can be left as empty / null if not used.</summary>
        public string Language { get; set; }

        /// <summary>Gets or sets a hint to the geocoder so that the results may be localised.</summary>
        public string Locale { get; set; }

        /// <summary>Gets or sets a hint to the geocoder so that the results may be localised.</summary>
        public string Region { get; set; }
    }
}
