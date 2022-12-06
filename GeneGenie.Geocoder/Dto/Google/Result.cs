// <copyright file="Result.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// A geocoded address and geometry information returned from Google.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Holds the separate parts of this address.
        /// </summary>
        public List<AddressComponent> Address_components { get; set; }

        /// <summary>
        /// The human-readable address of this location. This is a lossy format and could probably be missing some data.
        /// </summary>
        public string Formatted_address { get; set; }

        /// <summary>
        /// Contains positional data about this result.
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// A unique identifier that can be used with other Google APIs.
        /// </summary>
        public string Place_id { get; set; }

        /// <summary>
        /// This array contains a set of zero or more tags identifying the type of feature returned in the result.
        /// 
        /// For example, a geocode of "Chicago" returns "locality" which indicates that "Chicago" is a city,
        /// and also returns "political" which indicates it is a political entity.
        /// </summary>
        public List<string> Types { get; set; }
    }
}
