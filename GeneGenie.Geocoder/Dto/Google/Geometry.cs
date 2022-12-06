// <copyright file="Geometry.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// Contains positional data about an address result.
    /// 
    /// Full documentation is available at https://developers.google.com/maps/documentation/geocoding/requests-geocoding#results
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Optionally returned bounding box for the results. If this is populated then it is preferred instead of <see cref="Location"/>.
        /// </summary>
        public Bounds Bounds { get; set; }

        /// <summary>
        /// Contains the geocoded latitude and longitude value. For normal address lookups, this field is typically the most important.
        /// </summary>
        public LocationPair Location { get; set; }

        /// <summary>
        /// Stores additional data about the specified location.
        /// 
        /// The following values are currently supported:
        ///     "ROOFTOP" indicates that the returned result is a precise geocode for which we have location information accurate down to street address precision.
        ///     "RANGE_INTERPOLATED" indicates that the returned result reflects an approximation(usually on a road) interpolated between two precise points(such as intersections). Interpolated results are generally returned when rooftop geocodes are unavailable for a street address.
        ///     "GEOMETRIC_CENTER" indicates that the returned result is the geometric center of a result such as a polyline(for example, a street) or polygon(region).
        ///     "APPROXIMATE" indicates that the returned result is approximate.
        /// </summary>
        public string Location_type { get; set; }

        /// <summary>
        /// The recommended viewport for displaying the returned result, specified as two latitude and longitude values defining the
        /// southwest and northeast corner of the viewport bounding box.
        /// 
        /// Generally the viewport is used to frame a result when displaying it to a user.
        /// </summary>
        public Viewport Viewport { get; set; }
    }
}
