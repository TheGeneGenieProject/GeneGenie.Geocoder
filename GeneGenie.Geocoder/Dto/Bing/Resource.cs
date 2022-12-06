// <copyright file="Resource.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    /// <summary>
    /// A location resource returned from the Bing location API.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The postal address for the location returned.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Bounding box of the response. Structure [South Latitude, West Longitude, North Latitude, East Longitude].
        /// </summary>
        [JsonProperty("bbox")]
        public List<double> BoundingBox { get; set; }

        /// <summary>
        /// The level of confidence that the geocoded location result is a match. Can be High, Medium, Low.
        /// </summary>
        public string Confidence { get; set; }

        /// <summary>
        /// The classification of the geographic entity returned, such as Address.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// A collection of geocoded points that differ in how they were calculated and their suggested use.
        /// </summary>
        public List<Point> GeocodePoints { get; set; }

        /// <summary>
        /// One or more match code values that represent the geocoding level for each location in the response. Can be Good, Ambiguous, UpHierarchy.
        /// </summary>
        public List<string> MatchCodes { get; set; }

        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The latitude and longitude coordinates of the geocoded address.
        /// </summary>
        public Point Point { get; set; }
    }
}
