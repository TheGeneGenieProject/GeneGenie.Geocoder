// <copyright file="IGeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// An interface used to remove the geocoder dependency on HttpClient so that they can be
    /// tested without making calls to the real geocoders (Google, Bing etc. would ban us pretty
    /// quick if we ran a lot of tests against them).
    /// </summary>
    public interface IGeocoderHttpClient
    {
        /// <summary>
        /// For real geocoders this will make an API call via REST.
        /// For unit tested geocoders the url passed in would be examined
        /// in order to simulate a specific response.
        /// </summary>
        /// <param name="url">
        /// The full URL with parameters for an address geocode attempt, normally via GET.
        /// 
        /// Fake unit test geocoders receive specific values here which determines the results
        /// they'll pass back for simulation.
        /// </param>
        /// <returns>A HTTP response with the response from the API call.</returns>
        Task<HttpResponseMessage> MakeApiRequestAsync(string url);
    }
}
