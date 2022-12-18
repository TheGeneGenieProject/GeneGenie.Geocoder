// <copyright file="BingGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    /// <summary>
    /// A geocoder that calls the Bing locations API to generate coordinates for an address.
    /// </summary>
    public class BingGeocoder : IGeocoder
    {
        private const int MaxResults = 25;
        private const string BingRestApiEndpoint = "https://dev.virtualearth.net/REST/v1/Locations";

        private static readonly List<GeocoderStatusMapping> BingStatusMappings = new()
        {
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "OK", Status = GeocodeStatus.Success },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "Unauthorized", Status = GeocodeStatus.RequestDenied },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "Service Unavailable", Status = GeocodeStatus.TemporaryError },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = true, StatusText = "Unparseable error", Status = GeocodeStatus.Error },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "ZERO_RESULTS", Status = GeocodeStatus.ZeroResults },
        };

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
        }

        /// <inheritdoc/>
        public GeocoderNames GeocoderId { get => GeocoderNames.Bing; }

        private static readonly ResponseDetail OkResponse = new("OK", GeocodeStatus.Success);

        /// <inheritdoc/>
        public async Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var response = await GeocodeAddressInternalAsync(geocodeRequest);

            if (response.ResponseDetail.GeocodeStatus == GeocodeStatus.Success)
            {
                return new GeocodeResponseDto(response.ResponseDetail)
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

            return new GeocodeResponseDto(response.ResponseDetail);
        }

        private sealed class BingResponse : GeocoderAddressLookupResponse<RootResponse>
        {
            internal BingResponse(ResponseDetail responseStatusDetail) : base(responseStatusDetail)
            {
            }
        }

        /// <summary>
        /// Given an address will check it for validity, call the API and parse the most common error states.
        /// </summary>
        /// <param name="geocodeRequest">The address to lookup.</param>
        /// <returns>The status of the call, if OK then the content can be parsed for the results.</returns>
        private async Task<BingResponse> GeocodeAddressInternalAsync(GeocodeRequest geocodeRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(geocodeRequest.Address))
                {
                    logger.LogWarning((int)LogEventIds.GeocoderInputEmpty, "Data passed to geocoder was empty.");
                    return new BingResponse(new("Data passed to geocoder was empty.", GeocodeStatus.InvalidRequest));
                }

                var response = await MakeApiRequestAsync(geocodeRequest);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var geocodeStatus = HttpToGeocodeStatusCode(response.StatusCode);
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder HTTP error code of {ResponseDetail} resulted in a status of {GeocodeStatus}",
                        response.StatusCode.ToString(), geocodeStatus);
                    return new BingResponse(new(response.StatusCode.ToString(), geocodeStatus));
                }

                // Bing sends back this header instead of HTTP status 429 (back-off) when too many requests.
                if (response.Headers.Any(h => h.Key == "X-MS-BM-WS-INFO" && h.Value.Any(v => v == "1")))
                {
                    logger.LogWarning((int)LogEventIds.GeocoderTooManyRequests, "Backoff received from geocoder");
                    return new BingResponse(new(response.StatusCode.ToString(), GeocodeStatus.TooManyRequests));
                }

                var json = await response.Content.ReadAsStringAsync();
                logger.LogTrace((int)LogEventIds.GeocoderResponse, "Geocoder response was - {json}", json);
                var content = JsonConvert.DeserializeObject<RootResponse>(json);

                var responseDetail = ValidateResponse(content, geocodeRequest.Address);
                if (responseDetail.GeocodeStatus != GeocodeStatus.Success)
                {
                    return new BingResponse(responseDetail);
                }

                logger.LogTrace((int)LogEventIds.Success, "Geocode completed successfully for {address}.", geocodeRequest.Address);
                return new BingResponse(OkResponse)
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.GeocodeException, ex,
                    "Call to geocoder failed for {address} with error {exception}", geocodeRequest.Address, ex);

                return new BingResponse(new($"Call to geocoder failed for {geocodeRequest.Address} with error {ex}", GeocodeStatus.PermanentError));
            }
        }

        private static GeocodeStatus HttpToGeocodeStatusCode(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => GeocodeStatus.InvalidRequest,
                HttpStatusCode.Unauthorized => GeocodeStatus.RequestDenied,
                HttpStatusCode.Forbidden => GeocodeStatus.RequestDenied,
                HttpStatusCode.TooManyRequests => GeocodeStatus.TooManyRequests,
                HttpStatusCode.InternalServerError => GeocodeStatus.Error,
                HttpStatusCode.ServiceUnavailable => GeocodeStatus.TemporaryError,
                _ => GeocodeStatus.Error,
            };
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> MakeApiRequestAsync(GeocodeRequest geocodeRequest)
        {
            var url = BuildUrl(geocodeRequest);
            return await geocoderHttpClient.MakeApiRequestAsync(url);
        }

        /// <inheritdoc/>
        public ResponseDetail ValidateResponse(RootResponse content, string address)
        {
            if (content is null)
            {
                logger.LogCritical((int)LogEventIds.GeocoderReturnedNull, "Response missing from Bing geocode");
                return new("Response missing from Bing geocode", GeocodeStatus.PermanentError);
            }

            if (content.ErrorDetails != null && content.ErrorDetails.Any())
            {
                var errors = FormatErrors(content.ErrorDetails);
                logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder errors returned, {errors}", errors);
                return new(errors, GeocodeStatus.Error);
            }

            var status = LookupContentStatus(content.StatusDescription);
            if (status.IsPermanentError)
            {
                return LogHttpResponse("Geocoder returned permanent error", content.ErrorDetails, address, status, LogEventIds.GeocoderPermanentError);
            }

            if (status.IsTemporaryError)
            {
                return LogHttpResponse("Geocoder returned temporary error", content.ErrorDetails, address, status, LogEventIds.GeocoderTemporaryError);
            }

            if (content.ResourceSets == null)
            {
                status = LookupContentStatus("ZERO_RESULTS");
                return LogHttpResponse("Results missing from response", content.ErrorDetails, address, status, LogEventIds.GeocoderMissingResults);
            }

            if (!content.ResourceSets.Any() || content.ResourceSets.Sum(rs => rs.EstimatedTotal) == 0)
            {
                status = LookupContentStatus("ZERO_RESULTS");
                return LogHttpResponse("Zero results returned", content.ErrorDetails, address, status, LogEventIds.GeocoderZeroResults);
            }

            return ValidateGeometry(content.ResourceSets, address);
        }

        private ResponseDetail ValidateGeometry(List<ResourceSet> resourceSets, string address)
        {
            foreach (var resourceSet in resourceSets)
            {
                foreach (var resource in resourceSet.Resources)
                {
                    if (resource.BoundingBox is null)
                    {
                        return LogGeometryError("Missing bounding box from Bing geocode", address, LogEventIds.GeocoderMissingBounds);
                    }

                    if (resource.GeocodePoints is null || !resource.GeocodePoints.Any())
                    {
                        return LogGeometryError("Missing geocode points from Google geocode", address, LogEventIds.GeocoderMissingGeometry);
                    }

                    if (resource.Point is null)
                    {
                        return LogGeometryError("Missing point from Google geocode", address, LogEventIds.GeocoderMissingLocation);
                    }
                }
            }

            return OkResponse;
        }

        private ResponseDetail LogGeometryError(string prefix, string address, LogEventIds logEventId)
        {
            logger.LogWarning((int)logEventId, "Geometry error - {prefix} for {address}", prefix, address);

            var message = $"{prefix} for {address}";
            return new(message, GeocodeStatus.PermanentError);
        }

        private ResponseDetail LogHttpResponse(string prefix, List<string> errors, string address, GeocoderStatusMapping status, LogEventIds eventId)
        {
            var errorMessage = FormatErrors(errors);

            logger.LogWarning((int)eventId, "{prefix} for {address} with status of {status}, status description of {statusDescription} and error detail of {error}",
                prefix, address, status.Status.ToString(), status.StatusText, errorMessage);

            var message = $"{prefix} for {address} with status of {status}, status description of {status.StatusText} and error detail of {errorMessage}";
            return new(message, status.Status);
        }

        private static string FormatErrors(List<string> errors)
        {
            if (errors is null || !errors.Any())
            {
                return "";
            }

            return string.Join(", ", errors);
        }

        internal GeocoderStatusMapping LookupContentStatus(string status)
        {
            var statusRow = BingStatusMappings
                .FirstOrDefault(s => string.Compare(s.StatusText, status.Trim(), true) == 0);
            if (statusRow is null)
            {
                logger.LogWarning((int)LogEventIds.GeocoderUnknownContentStatus, "Unknown status of {status} returned from Google", status);
                return new GeocoderStatusMapping
                {
                    Status = GeocodeStatus.PermanentError,
                    StatusText = status,
                    IsPermanentError = true,
                };
            }

            return statusRow;
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
            if (points is { Count: 4 })
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
