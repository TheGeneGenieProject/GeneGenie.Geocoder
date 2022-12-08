// <copyright file="GeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// Plugs in to the geocoder classes so that they can make API calls to Google, Bing etc.
    /// Implements <see cref="IGeocoderHttpClient"/> so that unit tests can swap it out.
    /// </summary>
    public class GeocoderHttpClient : IGeocoderHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Creates a new instance of the geocoder client that wraps a HTTP client factory.
        /// </summary>
        /// <param name="httpClientFactory">An HTTP client factory from which we'll create connections.</param>
        public GeocoderHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Makes an API call from the geocoder class to a HTTP API.
        /// </summary>
        /// <param name="url">The full URL to issue a GET request to.</param>
        /// <returns>The HTTP response from the API.</returns>
        public async Task<HttpResponseMessage> MakeApiRequestAsync(string url)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            return response;
        }
    }
}
