// <copyright file="Response.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    /// <summary>
    /// The root object returned by Bing in response to a location API request.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// A status code that offers additional information about authentication success or failure.
        /// 
        /// One of the following values:
        ///     ValidCredentials
        ///     InvalidCredentials
        ///     CredentialsExpired
        ///     NotAuthorized
        ///     NoCredentials
        ///     None
        /// </summary>
        public string AuthenticationResultCode { get; set; }

        /// <summary>
        /// A collection of error descriptions. For example, ErrorDetails can identify parameter values that are not valid or missing.
        /// </summary>
        public List<string> ErrorDetails { get; set; }

        /// <summary>
        /// A collection of ResourceSet objects. A ResourceSet is a container of Resources returned by the request which for us, is location data.
        /// </summary>
        public List<ResourceSet> ResourceSets { get; set; }

        /// <summary>
        /// The HTTP Status code for the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// A description of the HTTP status code.
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// A unique identifier for the request.
        /// </summary>
        public string TraceId { get; set; }
    }
}
