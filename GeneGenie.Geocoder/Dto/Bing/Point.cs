// <copyright file="Point.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    public class Point
    {
        public List<double> BoundingBox { get; set; }

        public string CalculationMethod { get; set; }

        public List<double> Coordinates { get; set; }

        public List<string> UsageTypes { get; set; }
    }
}
