// <copyright file="GeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Neocoder.Interfaces;

    /// <summary>
    /// Plugs in to the geocoder classes so that they can make API calls to Google, Bing etc.
    /// Implements <see cref="IGeocoderHttpClient"/> so that unit tests can swap it out.
    /// </summary>
    public class GeocoderHttpClient : IGeocoderHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public GeocoderHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> MakeApiRequestAsync(string url)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            return response;
        }
    }
}
