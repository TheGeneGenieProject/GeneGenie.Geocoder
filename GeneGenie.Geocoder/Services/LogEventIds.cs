// <copyright file="LogEventIds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// Events logged by the geocoders to the standard logging framework.
    /// </summary>
    public enum LogEventIds
    {
        /// <summary>
        /// There should never be an event associated with this ID, if there is, some code has not set the status correctly.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Everything worked.
        /// </summary>
        Success = 1,

        /// <summary>
        /// The geocoder told us to slow down and stop querying it so fast.
        /// </summary>
        GeocoderTooManyRequests = 1001,

        /// <summary>
        /// Diagnostic log event for detailing the data sent back from the geocoder.
        /// </summary>
        GeocoderResponse = 1002,

        /// <summary>
        /// The geocoder service did not like what we sent it, more details in the log event.
        /// </summary>
        GeocoderError = 1003,

        /// <summary>
        /// The geocoder service returned a valid response but indicated that no addresses could be found. Ask the user to check the input data.
        /// </summary>
        GeocoderZeroResults = 1004,

        /// <summary>
        /// Something rather unexpected happened and we've not handled it well. If you see this, let the library authors know and we'll look into it.
        /// </summary>
        GeocodeException = 1005,

        /// <summary>
        /// The geocoder service was called and we were returned an empty result which we couldn't make sense of.
        /// </summary>
        GeocoderReturnedNull = 2002,

        /// <summary>
        /// Missing geometry data from geocoder response.
        /// </summary>
        GeocoderMissingGeometry = 2003,

        /// <summary>
        /// Missing bounds data from geocoder response.
        /// </summary>
        GeocoderMissingBounds = 2004,

        /// <summary>
        /// Missing location from geocoder response.
        /// </summary>
        GeocoderMissingLocation = 2005,
    }
}
