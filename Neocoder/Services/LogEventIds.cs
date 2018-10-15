// <copyright file="LogEventIds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Services
{
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

        GeocoderTooManyRequests = 1001,
        GeocoderResponse = 1002,
        GeocoderError = 1003,
        GeocoderZeroResults = 1004,
        GeocodeException = 1005,

        AddressKeyError = 2001,

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
