// <copyright file="Viewport.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// Contains the recommended viewport for displaying the returned result, specified as two latitude, longitude values
    /// defining the southwest and northeast corner of the viewport bounding box.
    /// 
    /// Generally the viewport is used to frame a result when displaying it to a user.
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// The North East point of the bounding box for this viewport.
        /// </summary>
        public LocationPair NorthEast { get; set; }

        /// <summary>
        /// The South West point of the bounding box for this viewport.
        /// </summary>
        public LocationPair SouthWest { get; set; }
    }
}
