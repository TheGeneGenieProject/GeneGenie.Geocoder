// <copyright file="Resource.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Resource
    {
        public Address Address { get; set; }

        [JsonProperty("bbox")]
        public List<double> BoundingBox { get; set; }

        public string Confidence { get; set; }

        public string EntityType { get; set; }

        public List<Point> GeocodePoints { get; set; }

        public List<string> MatchCodes { get; set; }

        public string Name { get; set; }

        public Point Point { get; set; }
    }
}
