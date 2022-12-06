// <copyright file="RootResponse.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// The root of an address lookup response from the Google API.
    /// 
    /// The docs below are a subset of https://developers.google.com/maps/documentation/geocoding/requests-geocoding
    /// which will be more up to date and accurate.
    /// </summary>
    public class RootResponse
    {
        /// <summary>
        /// When the geocoder returns a status code other than OK, there may be an additional error_message field within the Geocoding response object.
        /// This field contains more detailed information about the reasons behind the given status code.
        /// </summary>
        public string Error_message { get; set; }

        /// <summary>
        /// Indicates that the geocoder did not return an exact match for the original request, though it was able to match part of the requested address.
        /// </summary>
        public bool Partial_match { get; set; }

        /// <summary>
        /// An array of geocoded address information and geometry information.
        /// </summary>
        public List<Result> Results { get; set; }

        /// <summary>
        /// From the Google docs:
        /// The "status" field within the Geocoding response object contains the status of the request, and may contain debugging information
        /// to help you track down why geocoding is not working.
        /// 
        /// The "status" field may contain the following values:
        ///     "OK" indicates that no errors occurred; the address was successfully parsed and at least one geocode was returned.
        ///     "ZERO_RESULTS" indicates that the geocode was successful but returned no results. This may occur if the geocoder was passed a non-existent address.
        ///     "OVER_DAILY_LIMIT" indicates any of the following (see the Maps FAQ to learn how to fix this):
        ///         The API key is missing or invalid.
        ///         Billing has not been enabled on your account.
        ///         A self-imposed usage cap has been exceeded.
        ///         The provided method of payment is no longer valid (for example, a credit card has expired).
        ///     "OVER_QUERY_LIMIT" indicates that you are over your quota.
        ///     "REQUEST_DENIED" indicates that your request was denied.
        ///     "INVALID_REQUEST" generally indicates that the query (address, components or latlng) is missing.
        ///     "UNKNOWN_ERROR" indicates that the request could not be processed due to a server error. The request may succeed if you try again.
        /// </summary>
        public string Status { get; set; }
    }
}
