// <copyright file="Geometry.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    public class Geometry
    {
        /// <summary>
        /// Optionally returned bounding box for the results. If this is populated then it is preferred instead of <see cref="Location"/>.
        /// </summary>
        public Bounds Bounds { get; set; }

        public LocationPair Location { get; set; }

        public string Location_type { get; set; }

        public Viewport Viewport { get; set; }
    }
}
