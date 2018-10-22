// <copyright file="IGeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface used to remove the geocoder dependency on HttpClient so that they can be
    /// tested without making calls to the real geocoders (Google, Bing etc. would ban us pretty
    /// quick if we ran a lot of tests against them).
    /// </summary>
    public interface IGeocoderHttpClient
    {
        Task<HttpResponseMessage> MakeApiRequestAsync(string url);
    }
}
