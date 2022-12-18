// <copyright file="LogEventIds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// Events logged by the geocoders to the standard logging framework.
    /// Not all events are logged by all geocoders.
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

        /// <summary>
        /// Data passed to geocoder is null or whitespace and could not be used.
        /// </summary>
        GeocoderInputEmpty = 2006,

        /// <summary>
        /// The geocoder API returned an error that indicates an authorisation issue.
        /// This could be a bad API key, permissions issue or billing problem.
        /// You'll need to check the rest of the response and log into the
        /// geocoder control panel to troubleshoot fully.
        /// </summary>
        GeocoderApiCallDenied = 2007,

        /// <summary>
        /// An OK response was received from the API but the results were empty or Null.
        /// </summary>
        GeocoderMissingResults = 2008,

        /// <summary>
        /// Geocoder API could not process this query and is not likely to succeed if another attempt is made.
        /// Could be an issue with the addres, API key, billing etc.
        /// </summary>
        GeocoderPermanentError = 2009,

        /// <summary>
        /// Geocoder API could not process this query but might be able to in future.
        /// Could be due to too many requests, over quota, network conditions etc.
        /// </summary>
        GeocoderTemporaryError = 2010,

        /// <summary>
        /// The HTTP status came back as OK but the content of the response gave us a status code
        /// that we don't understand. The API may have changed or we've just not implemented everything.
        /// </summary>
        GeocoderUnknownContentStatus = 2011,
    }
}
