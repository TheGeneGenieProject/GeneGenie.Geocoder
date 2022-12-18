// <copyright file="GoogleGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    using GeneGenie.Geocoder.Dto.Google;

    /// <summary>
    /// A geocoder that calls the Google places API to generate coordinates for an address.
    /// </summary>
    public class GoogleGeocoder : IGeocoder
    {
        private const string GoogleRestApiEndpoint = "https://maps.googleapis.com/maps/api/geocode/json";

        private static readonly List<GeocoderStatusMapping> GoogleStatusMappings = new()
        {
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "OK", Status = GeocodeStatus.Success },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "INVALID_REQUEST", Status = GeocodeStatus.PermanentError },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "OVER_DAILY_LIMIT", Status = GeocodeStatus.TooManyRequests },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "OVER_QUERY_LIMIT", Status = GeocodeStatus.TooManyRequests },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "REQUEST_DENIED", Status = GeocodeStatus.PermanentError },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "UNKNOWN_ERROR", Status = GeocodeStatus.TemporaryError },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "ZERO_RESULTS", Status = GeocodeStatus.ZeroResults },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "Unparseable error", Status = GeocodeStatus.PermanentError },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "Content status empty", Status = GeocodeStatus.StatusEmpty },
        };

        private readonly IGeocoderHttpClient geocoderHttpClient;
        private readonly GeocoderSettings geocoderSettings;
        private readonly ILogger logger;

        /// <summary>
        /// Creates an instance of the Google geocoder.
        /// </summary>
        /// <param name="geocoderHttpClient">A class for making HTTP requests that can be swapped out in test.</param>
        /// <param name="geocoderSettings">The settings for this geocoder.</param>
        /// <param name="logger">A logger for creating diagnostic logs.</param>
        public GoogleGeocoder(IGeocoderHttpClient geocoderHttpClient, GeocoderSettings geocoderSettings, ILogger<GoogleGeocoder> logger)
        {
            this.geocoderHttpClient = geocoderHttpClient;
            this.geocoderSettings = geocoderSettings;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public GeocoderNames GeocoderId { get => GeocoderNames.Google; }

        private static readonly ResponseDetail OkResponse = new("OK", GeocodeStatus.Success);

        /// <inheritdoc/>
        public async Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            var response = await GeocodeAddressInternalAsync(geocodeRequest);

            if (response.ResponseDetail.GeocodeStatus == GeocodeStatus.Success)
            {
                return new GeocodeResponseDto(OkResponse)
                {
                    Locations = response.Content
                        .Results
                        .Select(r => new GeocodeLocationDto
                        {
                            Bounds = ConvertBounds(r.Geometry),
                            FormattedAddress = r.Formatted_address,
                            Location = ConvertLocation(r.Geometry.Location),
                        })
                        .ToList(),
                };
            }

            return new GeocodeResponseDto(response.ResponseDetail);
        }

        private sealed class GoogleResponse : GeocoderAddressLookupResponse<RootResponse>
        {
            internal GoogleResponse(ResponseDetail responseStatusDetail) : base(responseStatusDetail)
            {
            }
        }

        private async Task<GoogleResponse> GeocodeAddressInternalAsync(GeocodeRequest geocodeRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(geocodeRequest.Address))
                {
                    logger.LogWarning((int)LogEventIds.GeocoderInputEmpty, "Data passed to geocoder was empty.");
                    return new GoogleResponse(new("Data passed to geocoder was empty.", GeocodeStatus.InvalidRequest));
                }

                var response = await MakeApiRequestAsync(geocodeRequest);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder HTTP error code of {responseDetail}", response.StatusCode.ToString());
                    return new GoogleResponse(new(response.StatusCode.ToString(), GeocodeStatus.Error));
                }

                var json = await response.Content.ReadAsStringAsync();
                logger.LogTrace((int)LogEventIds.GeocoderResponse, "Geocoder response was - {json}", json);
                var content = JsonConvert.DeserializeObject<RootResponse>(json);

                var responseDetail = ValidateResponse(content, geocodeRequest.Address);
                if (responseDetail.GeocodeStatus != GeocodeStatus.Success)
                {
                    return new GoogleResponse(responseDetail);
                }

                logger.LogTrace((int)LogEventIds.Success, "Geocode completed successfully for {address}.", geocodeRequest.Address);
                return new GoogleResponse(OkResponse)
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.GeocodeException, ex,
                    "Call to geocoder failed for {address} with error {exception}", geocodeRequest.Address, ex);

                return new GoogleResponse(new($"Call to geocoder failed for {geocodeRequest.Address} with error {ex}", GeocodeStatus.PermanentError));
            }
        }

        internal GeocoderStatusMapping LookupContentStatus(RootResponse content)
        {
            var statusRow = GoogleStatusMappings
                .FirstOrDefault(s => string.Compare(s.StatusText, content.Status.Trim(), true) == 0);
            if (statusRow is null)
            {
                logger.LogWarning((int)LogEventIds.GeocoderUnknownContentStatus, "Unknown status of {status} returned from Google", content.Status);
                return new GeocoderStatusMapping
                {
                    Status = GeocodeStatus.PermanentError,
                    StatusText = content.Status,
                    IsPermanentError = true,
                };
            }

            return statusRow;
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
                logger.LogCritical((int)LogEventIds.GeocoderReturnedNull, "Response missing from Google geocode");
                return new("Response missing from Google geocode", GeocodeStatus.PermanentError);
            }

            var status = LookupContentStatus(content);
            if (status.IsPermanentError)
            {
                return LogHttpResponse("Geocoder returned permanent error", content.Error_message, address, status, LogEventIds.GeocoderPermanentError);
            }

            if (status.IsTemporaryError)
            {
                return LogHttpResponse("Geocoder returned temporary error", content.Error_message, address, status, LogEventIds.GeocoderTemporaryError);
            }

            if (status.Status != GeocodeStatus.Success)
            {
                return LogHttpResponse("Geocoder did not return success but did not raise an error either when searching", content.Error_message, address, status, LogEventIds.GeocoderZeroResults);
            }

            if (content.Results == null)
            {
                status = new GeocoderStatusMapping { Status = GeocodeStatus.ZeroResults, StatusText = "Results null when searching" };
                return LogHttpResponse("Results null when searching", content.Error_message, address, status, LogEventIds.GeocoderMissingResults);
            }

            if (!content.Results.Any())
            {
                status = new GeocoderStatusMapping { Status = GeocodeStatus.ZeroResults, StatusText = "Results missing when searching" };
                return LogHttpResponse("Results missing when searching", content.Error_message, address, status, LogEventIds.GeocoderZeroResults);
            }

            return ValidateGeometry(content.Results, address);
        }

        /// <summary>
        /// Takes the error message and status of an API call and logs it as a warning, then returns response details
        /// for the end user to view.
        /// </summary>
        /// <param name="prefix">Text to show before the error parameters.</param>
        /// <param name="errorMessage">Message received from the API.</param>
        /// <param name="address">The address we were trying to geocode.</param>
        /// <param name="status">The status we want to return to the end user.</param>
        /// <param name="eventId">The event id for the logging output.</param>
        /// <returns></returns>
        private ResponseDetail LogHttpResponse(string prefix, string errorMessage, string address, GeocoderStatusMapping status, LogEventIds eventId)
        {
            logger.LogWarning((int)eventId, "{prefix} for {address} with status of {status}, status description of {statusDescription} and error detail of {error}",
                prefix, address, status.Status.ToString(), status.StatusText, errorMessage);

            var message = $"{prefix} for {address} with status of {status}, status description of {status.StatusText} and error detail of {errorMessage}";
            return new(message, status.Status);
        }

        private ResponseDetail ValidateGeometry(List<Result> results, string address)
        {
            if (results.Any(r => r.Geometry == null))
            {
                return LogGeometryError("Missing geometry from Google geocode", address, LogEventIds.GeocoderMissingGeometry);
            }

            if (results.Any(r => r.Geometry.Bounds == null && r.Geometry.Viewport == null))
            {
                return LogGeometryError("Missing geometry from Google geocode", address, LogEventIds.GeocoderMissingBounds);
            }

            if (results.Any(r => r.Geometry.Location == null))
            {
                return LogGeometryError("Missing locations from Google geocode", address, LogEventIds.GeocoderMissingLocation);
            }

            return OkResponse;
        }

        private ResponseDetail LogGeometryError(string prefix, string address, LogEventIds logEventId)
        {
            logger.LogWarning((int)logEventId, "Geometry error - {prefix} for {address}", prefix, address);

            var message = $"{prefix} for {address}";
            return new(message, GeocodeStatus.PermanentError);
        }

        private static Models.Geo.LocationPair ConvertLocation(LocationPair locationPair)
        {
            return new Models.Geo.LocationPair
            {
                Latitude = locationPair.Lat,
                Longitude = locationPair.Lng,
            };
        }

        private static Models.Geo.Bounds ConvertBounds(Geometry geometry)
        {
            if (geometry.Bounds != null)
            {
                return new Models.Geo.Bounds
                {
                    NorthEast = ConvertLocation(geometry.Bounds.NorthEast),
                    SouthWest = ConvertLocation(geometry.Bounds.SouthWest),
                };
            }

            return new Models.Geo.Bounds
            {
                NorthEast = ConvertLocation(geometry.Viewport.NorthEast),
                SouthWest = ConvertLocation(geometry.Viewport.SouthWest),
            };
        }

        private string BuildUrl(GeocodeRequest geocodeRequest)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "address", geocodeRequest.Address ?? string.Empty },
                { "key", geocoderSettings.ApiKey ?? string.Empty },
                { "sensor", "false" },
            };

            GeocoderUrlHelper.AddUrlParameters(geocodeRequest, parameters, "language", "region", "bounds");

            return QueryHelpers.AddQueryString(GoogleRestApiEndpoint, parameters);
        }
    }
}
