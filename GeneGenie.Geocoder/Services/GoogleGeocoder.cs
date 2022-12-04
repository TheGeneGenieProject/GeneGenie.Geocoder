// <copyright file="GoogleGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Dto;
    using GeneGenie.Geocoder.Dto.Google;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models;
    using GeneGenie.Geocoder.Models.Geo;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class GoogleGeocoder : IGeocoder
    {
        private const string GoogleRestApiEndpoint = "https://maps.googleapis.com/maps/api/geocode/json";

        private static readonly List<GeocoderStatusMapping> GoogleStatusMappings = new List<GeocoderStatusMapping>
        {
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "OK", Status = GeocodeStatus.Success },
            new GeocoderStatusMapping { IsPermanentError = true, IsTemporaryError = false, StatusText = "INVALID_REQUEST", Status = GeocodeStatus.InvalidRequest },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "OVER_QUERY_LIMIT", Status = GeocodeStatus.TooManyRequests },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "REQUEST_DENIED", Status = GeocodeStatus.RequestDenied },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = true, StatusText = "UNKNOWN_ERROR", Status = GeocodeStatus.Error },
            new GeocoderStatusMapping { IsPermanentError = false, IsTemporaryError = false, StatusText = "ZERO_RESULTS", Status = GeocodeStatus.ZeroResults },
        };

        private readonly IGeocoderHttpClient geocoderHttpClient;
        private readonly GeocoderSettings geocoderSettings;
        private readonly ILogger logger;

        public GoogleGeocoder(IGeocoderHttpClient geocoderHttpClient, GeocoderSettings geocoderSettings, ILogger<GoogleGeocoder> logger)
        {
            this.geocoderHttpClient = geocoderHttpClient;
            this.geocoderSettings = geocoderSettings;
            this.logger = logger;
        }

        public GeocoderNames GeocoderId { get => GeocoderNames.Google; }

        public async Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(geocodeRequest.Address))
                {
                    logger.LogWarning((int)LogEventIds.GeocoderInputEmpty, "Data passed to geocoder was empty.");
                    return new GeocodeResponseDto(GeocodeStatus.InvalidRequest);
                }

                var response = await MakeApiRequestAsync(geocodeRequest);

                var httpStatus = ValidateHttpResponse(response);
                if (httpStatus != GeocodeStatus.Success)
                {
                    return new GeocodeResponseDto(httpStatus);
                }

                var json = await response.Content.ReadAsStringAsync();
                logger.LogTrace((int)LogEventIds.GeocoderResponse, json);
                var content = JsonConvert.DeserializeObject<RootObject>(json);

                var statusCode = ValidateResponse(content);
                if (statusCode != GeocodeStatus.Success)
                {
                    return new GeocodeResponseDto(statusCode);
                }

                var status = ExtractStatus(content.Status);
                if (status.IsPermanentError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned permanent error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, content.Error_message);
                    return new GeocodeResponseDto(GeocodeStatus.Error);
                }

                if (status.IsTemporaryError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned temporary error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, content.Error_message);
                    return new GeocodeResponseDto(GeocodeStatus.Error);
                }

                logger.LogTrace((int)LogEventIds.Success, "Geocode completed successfully for {address}.", geocodeRequest.Address);
                return new GeocodeResponseDto(GeocodeStatus.Success)
                {
                    Locations = content
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
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.GeocodeException, ex, "Call to geocoder failed for {address}", geocodeRequest?.Address);
                return new GeocodeResponseDto(GeocodeStatus.Error);
            }
        }

        public GeocoderStatusMapping ExtractStatus(string statusText)
        {
            if (string.IsNullOrWhiteSpace(statusText))
            {
                return GoogleStatusMappings.First(s => s.Status == GeocodeStatus.Error);
            }

            var statusRow = GoogleStatusMappings.FirstOrDefault(s => s.StatusText == statusText.Trim());
            if (statusRow == null)
            {
                return GoogleStatusMappings.First(s => s.Status == GeocodeStatus.Error);
            }

            return statusRow;
        }

        /// <summary>
        /// Validates the HTTP level response (the status code and headers).
        /// Does not validate the message itself which is handled by <see cref="ValidateResponse(Response)"/>.
        /// </summary>
        /// <param name="response">The HTTP response to validate.</param>
        /// <returns>Returns <see cref="GeocodeStatus.Success"/> if all OK, otherwise returns an error code.</returns>
        private GeocodeStatus ValidateHttpResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service unavailable");
                return GeocodeStatus.TemporaryError;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service error");
                return GeocodeStatus.Error;
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogWarning((int)LogEventIds.GeocoderError, "Service error, status code {statusCode}", response.StatusCode);
                return GeocodeStatus.Error;
            }

            return GeocodeStatus.Success;
        }

        private async Task<HttpResponseMessage> MakeApiRequestAsync(GeocodeRequest geocodeRequest)
        {
            var url = BuildUrl(geocodeRequest);
            return await geocoderHttpClient.MakeApiRequestAsync(url);
        }

        private GeocodeStatus ValidateResponse(RootObject root)
        {
            var statusCode = ValidateResults(root);
            if (statusCode != GeocodeStatus.Success)
            {
                return statusCode;
            }

            return ValidateGeometry(root.Results);
        }

        private GeocodeStatus ValidateResults(RootObject root)
        {
            if (root == null || root.Results == null || !root.Results.Any())
            {
                if (root?.Status == "ZERO_RESULTS")
                {
                    // Technically, the call worked as far as the API is concerned. There were just no results.
                    logger.LogInformation((int)LogEventIds.GeocoderZeroResults, "Zero results received from Google geocode");
                    return GeocodeStatus.ZeroResults;
                }

                logger.LogCritical((int)LogEventIds.GeocoderReturnedNull, "Results missing from Google geocode");
                return GeocodeStatus.Error;
            }

            return GeocodeStatus.Success;
        }

        private GeocodeStatus ValidateGeometry(List<Result> results)
        {
            if (results.Any(r => r.Geometry == null))
            {
                logger.LogCritical((int)LogEventIds.GeocoderMissingGeometry, "Missing geometry from Google geocode");
                return GeocodeStatus.Error;
            }

            if (results.Any(r => r.Geometry.Bounds == null && r.Geometry.Viewport == null))
            {
                logger.LogCritical((int)LogEventIds.GeocoderMissingBounds, "Missing geometry from Google geocode");
                return GeocodeStatus.Error;
            }

            if (results.Any(r => r.Geometry.Location == null))
            {
                logger.LogCritical((int)LogEventIds.GeocoderMissingLocation, "Missing locations from Google geocode");
                return GeocodeStatus.Error;
            }

            return GeocodeStatus.Success;
        }

        private Models.Geo.LocationPair ConvertLocation(Dto.Google.LocationPair locationPair)
        {
            return new Models.Geo.LocationPair
            {
                Latitude = locationPair.Lat,
                Longitude = locationPair.Lng,
            };
        }

        private Models.Geo.Bounds ConvertBounds(Geometry geometry)
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

            if (!string.IsNullOrWhiteSpace(geocodeRequest.Language))
            {
                parameters.Add("language", geocodeRequest.Language);
            }

            if (!string.IsNullOrWhiteSpace(geocodeRequest.Region))
            {
                parameters.Add("region", geocodeRequest.Region);
            }

            if (geocodeRequest.BoundsHint != null)
            {
                var sw = $"{geocodeRequest.BoundsHint.SouthWest.Latitude},{geocodeRequest.BoundsHint.SouthWest.Longitude}";
                var ne = $"{geocodeRequest.BoundsHint.NorthEast.Latitude},{geocodeRequest.BoundsHint.NorthEast.Longitude}";
                parameters.Add("bounds", $"{sw}|{ne}");
            }

            return QueryHelpers.AddQueryString(GoogleRestApiEndpoint, parameters);
        }
    }
}
