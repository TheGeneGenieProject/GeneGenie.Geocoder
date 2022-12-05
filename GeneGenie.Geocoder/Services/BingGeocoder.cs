// <copyright file="BingGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    public class BingGeocoder : IGeocoder, IGeocoderAddressProcessor<Response>
    {
        private const int MaxResults = 25;
        private const string BingRestApiEndpoint = "https://dev.virtualearth.net/REST/v1/Locations";

        private static readonly List<GeocoderStatusMapping> BingStatusMappings = new List<GeocoderStatusMapping>
        {
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "OK", Status = GeocodeStatus.Success },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "Unauthorized", Status = GeocodeStatus.Error },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "Service Unavailable", Status = GeocodeStatus.TemporaryError },
        };

        private readonly IGeocoderHttpClient geocoderHttpClient;
        private readonly GeocoderSettings geocoderSettings;
        private readonly ILogger logger;

        public BingGeocoder(IGeocoderHttpClient geocoderHttpClient, GeocoderSettings geocoderSettings, ILogger<BingGeocoder> logger)
        {
            this.geocoderHttpClient = geocoderHttpClient;
            this.geocoderSettings = geocoderSettings;
            this.logger = logger;
        }

        public GeocoderNames GeocoderId { get => GeocoderNames.Bing; }

        public async Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var geocoderAddressLookup = new GeocoderAddressLookup<Response>(this, logger);

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

        public GeocoderStatusMapping ExtractStatus(Response content)
        {
            if (string.IsNullOrWhiteSpace(content.StatusDescription))
            {
                return new GeocoderStatusMapping
                {
                    Status = GeocodeStatus.Error,
                    StatusText = "Empty status received from Bing, unable to parse response.",
                };
            }

            var statusRow = BingStatusMappings.FirstOrDefault(s => s.StatusText == content.StatusDescription.Trim());
            if (statusRow == null)
            {
                statusRow = new GeocoderStatusMapping
                {
                    Status = GeocodeStatus.Error,
                    StatusText = content.StatusDescription,
                };
            }

            return statusRow;
        }

        /// <summary>
        /// Validates the HTTP level response (the status code and headers).
        /// Does not validate the message itself which is handled by <see cref="ValidateResponse(Response)"/>.
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
                // TODO: Need to tie the address being backed off from in via some kind of correlation ID.
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

        public async Task<HttpResponseMessage> MakeApiRequestAsync(GeocodeRequest geocodeRequest)
        {
            var url = BuildUrl(geocodeRequest);
            return await geocoderHttpClient.MakeApiRequestAsync(url);
        }

        public GeocodeStatus ValidateResponse(Response content)
        {
            if (content is null)
            {
                logger.LogWarning((int)LogEventIds.GeocoderNullResponse, "Null response received from API.");
                return GeocodeStatus.Error;
            }

            if (content.ErrorDetails != null && content.ErrorDetails.Any())
            {
                content.ErrorDetails.ForEach(e => logger.LogWarning((int)LogEventIds.GeocoderError, e));
                return GeocodeStatus.Error;
            }

            if (content.ResourceSets == null || !content.ResourceSets.Any() || content.ResourceSets.Sum(rs => rs.EstimatedTotal) == 0)
            {
                // TODO: See how we can associate this with a request ID so we can see why.
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

            if (!string.IsNullOrWhiteSpace(geocodeRequest.Locale))
            {
                parameters.Add("c", geocodeRequest.Locale);
            }

            if (!string.IsNullOrWhiteSpace(geocodeRequest.Region))
            {
                parameters.Add("userRegion", geocodeRequest.Region);
            }

            if (geocodeRequest.BoundsHint != null)
            {
                var sw = $"{geocodeRequest.BoundsHint.SouthWest.Latitude},{geocodeRequest.BoundsHint.SouthWest.Longitude}";
                var ne = $"{geocodeRequest.BoundsHint.NorthEast.Latitude},{geocodeRequest.BoundsHint.NorthEast.Longitude}";
                parameters.Add("userMapView", $"{sw},{ne}");
            }

            return QueryHelpers.AddQueryString(BingRestApiEndpoint, parameters);
        }

        private LocationPair ConvertPoint(Point point)
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

        private Bounds ConvertBoundingBox(List<double> points)
        {
            if (points != null && points.Count == 4)
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
