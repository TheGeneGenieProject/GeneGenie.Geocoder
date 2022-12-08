// <copyright file="FakeGeocoderHttpClient.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    /// <summary>
    /// Fake for testing geocoder classes that implement <see cref="IGeocoder"/> and make network calls.
    /// This class replaces those network calls and does not cause any network traffic.
    /// </summary>
    public class FakeGeocoderHttpClient : IGeocoderHttpClient
    {
        /// <summary>
        /// Simulates a HTTP call to an external geocoding service and returns a result based on the
        /// test parameters passed in.
        /// </summary>
        /// <param name="url">Parameterised tokens representing the output the calling test would like to see.</param>
        /// <returns>Simulated HTTP response as defined by the test input parameters.</returns>
        public async Task<HttpResponseMessage> MakeApiRequestAsync(string url)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
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

            // Google uses 'address' for the parameter.
            var address = keyvalues.FirstOrDefault(k => k.StartsWith("address"));
            if (string.IsNullOrWhiteSpace(address))
            {
                // Could not find 'address', check if this is a Bing fake query.
                address = keyvalues.FirstOrDefault(k => k.StartsWith("query"));
                if (string.IsNullOrWhiteSpace(address))
                {
                    // Could not find 'address', check if this is a Bing fake query.
                    return HttpStatusCode.NotImplemented;
                }
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
