// <copyright file="Bounds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// The bounding box which can fully contain a returned result.
    /// 
    /// Note that these bounds may not match the recommended viewport.
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// The North East point of this bounding box.
        /// </summary>
        public LocationPair NorthEast { get; set; }

        /// <summary>
        /// The South West point of this bounding box.
        /// </summary>
        public LocationPair SouthWest { get; set; }
    }
}
