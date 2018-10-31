// <copyright file="FakeGeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Tests.ExtensionMethods;

    /// <summary>
    /// Fake for testing the <see cref="GoogleGeocoder"/> class without causing any network traffic.
    /// </summary>
    public class FakeGeocoderHttpClient : IGeocoderHttpClient
    {
        public async Task<HttpResponseMessage> MakeApiRequestAsync(string url)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("Invalid response"),
            };

            if (url.Contains("NULL"))
            {
                response.Content = new StringContent(string.Empty);
            }
            else if (url.Contains("Header") && url.Contains("SlowDown"))
            {
                response.Headers.Add("X-MS-BM-WS-INFO", "1");
            }
            else if (url.Contains("HttpStatusCode"))
            {
                response.StatusCode = ExtractStatusCodeFromUrl(url);
            }
            else if (url.Contains("File"))
            {
                response.Content = new StringContent(ExtractContentFromFile(url));
            }

            return await Task.FromResult(response);
        }

        private HttpStatusCode ExtractStatusCodeFromUrl(string url)
        {
            var urlParams = url.Substring(url.IndexOf("?") + 1);
            var keyvalues = urlParams.Split("&");

            var address = keyvalues.FirstOrDefault(k => k.StartsWith("address"));
            if (string.IsNullOrWhiteSpace(address))
            {
                return HttpStatusCode.NotImplemented;
            }

            // String is of the form address=httpstatuscode=value.
            var statusText = WebUtility.UrlDecode(address).Split("=")[2];
            if (int.TryParse(statusText, out var statusValue))
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), statusValue))
                {
                    return (HttpStatusCode)statusValue;
                }
            }

            if (Enum.IsDefined(typeof(HttpStatusCode), statusText))
            {
                if (Enum.TryParse<HttpStatusCode>(statusText, out var enumStatus))
                {
                    return enumStatus;
                }
            }

            return HttpStatusCode.NotImplemented;
        }

        private string ExtractContentFromFile(string url)
        {
            var content = string.Empty;
            var urlParams = url.Substring(url.IndexOf("?") + 1);
            var keyvalues = urlParams.Split("&");

            foreach (var keyvalueItem in keyvalues)
            {
                var pair = keyvalueItem.Split("=");

                if (pair.Length == 2 && pair[1].StartsWith("File"))
                {
                    var cleaned = WebUtility.UrlDecode(pair[1]);
                    var fileSplit = cleaned.Split("=");

                    content = ResourceReader.ReadEmbeddedFile($"GeneGenie.Geocoder.Tests/Data/{fileSplit[1]}");
                    break;
                }
            }

            return content;
        }
    }
}
