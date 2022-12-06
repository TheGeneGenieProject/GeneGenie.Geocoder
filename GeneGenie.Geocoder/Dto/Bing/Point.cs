// <copyright file="Point.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    /// <summary>
    /// Represents a point on the surface of the earth.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// The method that was used to compute the geocode point.
        /// 
        /// Can be one of the following values:
        ///     Interpolation: The geocode point was matched to a point on a road using interpolation.
        ///     InterpolationOffset: The geocode point was matched to a point on a road using interpolation with an additional offset to shift the point to the side of the street.
        ///     Parcel: The geocode point was matched to the center of a parcel.
        ///     Rooftop: The geocode point was matched to the rooftop of a building.
        /// </summary>
        public string CalculationMethod { get; set; }

        /// <summary>
        /// The coordinate information of the point [Latitude,Longitude].
        /// </summary>
        public List<double> Coordinates { get; set; }

        /// <summary>
        /// The best use for the geocode point. Can be Display or Route. 
        /// Each geocode point is defined as a Route point, a Display point or both.
        /// Use Route points if you are creating a route to the location.
        /// 
        /// Use Display points if you are showing the location on a map. 
        /// 
        /// For example, if the location is a park, a Route point may specify an entrance to the park where you can enter with a car, 
        /// and a Display point may be a point that specifies the center of the park.
        /// </summary>
        public List<string> UsageTypes { get; set; }
    }
}
