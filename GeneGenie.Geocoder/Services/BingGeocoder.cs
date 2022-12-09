// <copyright file="BingGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// A geocoder that calls the Bing locations API to generate coordinates for an address.
    /// </summary>
    public class BingGeocoder : IGeocoder, IGeocoderAddressProcessor<RootResponse>
    {
        private const int MaxResults = 25;
        private const string BingRestApiEndpoint = "https://dev.virtualearth.net/REST/v1/Locations";

        private static readonly List<GeocoderStatusMapping> BingStatusMappings = new List<GeocoderStatusMapping>
        {
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "OK", Status = GeocodeStatus.Success },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "Unauthorized", Status = GeocodeStatus.RequestDenied },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "Service Unavailable", Status = GeocodeStatus.TemporaryError },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = true, StatusText = "Unparseable error", Status = GeocodeStatus.Error },
        };

        private readonly GeocoderAddressLookup<RootResponse> geocoderAddressLookup;
        private readonly IGeocoderHttpClient geocoderHttpClient;
        private readonly GeocoderSettings geocoderSettings;
        private readonly ILogger logger;

        /// <summary>
        /// Creates an instance of the Bing geocoder.
        /// </summary>
        /// <param name="geocoderHttpClient">A class for making HTTP requests that can be swapped out in test.</param>
        /// <param name="geocoderSettings">The settings for this geocoder.</param>
        /// <param name="logger">A logger for creating diagnostic logs.</param>
        public BingGeocoder(IGeocoderHttpClient geocoderHttpClient, GeocoderSettings geocoderSettings, ILogger<BingGeocoder> logger)
        {
            this.geocoderHttpClient = geocoderHttpClient;
            this.geocoderSettings = geocoderSettings;
            this.logger = logger;
            geocoderAddressLookup = new GeocoderAddressLookup<RootResponse>(this, logger);
        }

        /// <inheritdoc/>
        public GeocoderNames GeocoderId { get => GeocoderNames.Bing; }

        /// <inheritdoc/>
        public async Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var response = await geocoderAddressLookup.GeocodeAddressAsync(geocodeRequest);

            if (response.ResponseStatus == GeocodeStatus.Success)
            {
                return new GeocodeResponseDto(GeocodeStatus.Success)
                {
                    Locations = response.Content
                        .ResourceSets
                        .SelectMany(rs => rs
                            .Resources
                            .Select(r => new GeocodeLocationDto
                            {
                                Bounds = ConvertBoundingBox(r.BoundingBox),
                                FormattedAddress = r.Address.FormattedAddress,
                                Location = ConvertPoint(r.Point),
                            }))
                        .ToList(),
                };
            }

            return new GeocodeResponseDto(response.ResponseStatus);
        }

        /// <inheritdoc/>
        public GeocoderStatusMapping ExtractStatus(RootResponse content)
        {
            if (string.IsNullOrWhiteSpace(content.StatusDescription))
            {
                return new GeocoderStatusMapping
                {
                    Status = GeocodeStatus.Error,
                    StatusText = "Empty status received from Bing, unable to parse response.",
                };
            }

            var statusRow = BingStatusMappings
                .FirstOrDefault(s => string.Compare(s.StatusText, content.StatusDescription.Trim(), true) == 0);
            if (statusRow == null)
            {
                return BingStatusMappings.First(s => s.Status == GeocodeStatus.Error);
            }

            return statusRow;
        }

        /// <summary>
        /// Validates the HTTP level response (the status code and headers).
        /// Does not validate the message itself which is handled by <see cref="ValidateResponse(RootResponse)"/>.
        /// </summary>
        /// <param name="response">The HTTP response to validate.</param>
        /// <returns>Returns <see cref="GeocodeStatus.Success"/> if all OK, otherwise returns an error code.</returns>
        public GeocodeStatus ValidateHttpResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service unavailable");
                return GeocodeStatus.TemporaryError;
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service error");
                return GeocodeStatus.Error;
            }

            // Bing sends back this header instead of HTTP status 429 (back-off) when too many requests.
            if (response.Headers.Any(h => h.Key == "X-MS-BM-WS-INFO" && h.Value.Any(v => v == "1")))
            {
                logger.LogWarning((int)LogEventIds.GeocoderTooManyRequests, "Backoff received from geocoder");
                return GeocodeStatus.TooManyRequests;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service error, status code {statusCode}", response.StatusCode);
                return GeocodeStatus.Error;
            }

            return GeocodeStatus.Success;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> MakeApiRequestAsync(GeocodeRequest geocodeRequest)
        {
            var url = BuildUrl(geocodeRequest);
            return await geocoderHttpClient.MakeApiRequestAsync(url);
        }

        /// <inheritdoc/>
        public GeocodeStatus ValidateResponse(RootResponse content)
        {
            if (content is null)
            {
                logger.LogWarning((int)LogEventIds.GeocoderReturnedNull, "Null response received from API.");
                return GeocodeStatus.Error;
            }

            if (content.ErrorDetails != null && content.ErrorDetails.Any())
            {
                content.ErrorDetails.ForEach(e => logger.LogWarning((int)LogEventIds.GeocoderError, e));
                return GeocodeStatus.Error;
            }

            if (content.ResourceSets == null || !content.ResourceSets.Any() || content.ResourceSets.Sum(rs => rs.EstimatedTotal) == 0)
            {
                logger.LogInformation((int)LogEventIds.GeocoderZeroResults, "Zero results received.");
                return GeocodeStatus.ZeroResults;
            }

            if (content.StatusCode != (int)HttpStatusCode.OK)
            {
                logger.LogInformation((int)LogEventIds.GeocoderZeroResults, "Results received but status code was not 'OK', it was instead '{statusCode}'.", content.StatusCode);
                return GeocodeStatus.Error;
            }

            return GeocodeStatus.Success;
        }

        private string BuildUrl(GeocodeRequest geocodeRequest)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "query", geocodeRequest.Address ?? string.Empty },
                { "key", geocoderSettings.ApiKey ?? string.Empty },
                { "maxResults", MaxResults.ToString() },
            };

            GeocoderUrlHelper.AddUrlParameters(geocodeRequest, parameters, "c", "userRegion", "userMapView");

            return QueryHelpers.AddQueryString(BingRestApiEndpoint, parameters);
        }

        private static LocationPair ConvertPoint(Point point)
        {
            if (point != null && point.Coordinates.Count == 2)
            {
                return new LocationPair
                {
                    Latitude = point.Coordinates[0],
                    Longitude = point.Coordinates[1],
                };
            }

            return null;
        }

        private static Bounds ConvertBoundingBox(List<double> points)
        {
            if (points is {Count: 4})
            {
                return new Bounds
                {
                    NorthEast = new LocationPair { Latitude = points[2], Longitude = points[3] },
                    SouthWest = new LocationPair { Latitude = points[0], Longitude = points[1] },
                };
            }

            return null;
        }
    }
}
