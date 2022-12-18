// <copyright file="GeocodeStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// The status of an address lookup request as returned by a geocoder service.
    /// </summary>
    public enum GeocodeStatus
    {
        /// <summary>Should never return this status, it is here to catch code that does not initialise properly.</summary>
        NotSet,

        /// <summary>Indicates that the request could not be processed due to a server error. The request may succeed if you try again.</summary>
        Error,

        /// <summary>Indicates that no errors occurred; the address was successfully parsed and at least one geocode was returned.</summary>
        Success,

        /// <summary>Generally indicates that the query (address, components or latlng) is missing.</summary>
        InvalidRequest,

        /// <summary>Indicates that you are querying the server too fast and should rate limit your queries.</summary>
        TooManyRequests,

        /// <summary>Indicates that your request was denied (possibly due to a permission issue).</summary>
        RequestDenied,

        /// <summary>Indicates that the geocode was successful but returned no results. This may occur if the geocoder was passed a non - existent address.</summary>
        ZeroResults,

        /// <summary>The server is temporarily unavailable, usually due to high load or maintenance.</summary>
        TemporaryError,

        /// <summary>Indicates that the request could not be processed due to a server error and probably won't work without user attention.</summary>
        PermanentError,

        /// <summary>The status code text returned in the content from the API was blank and is unusable.</summary>
        StatusEmpty,
    }
}
