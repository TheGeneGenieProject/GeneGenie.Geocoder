// <copyright file="BingGeocoder.cs" company="GeneGenie.com">
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
    using GeneGenie.Geocoder.Dto.Bing;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models;
    using GeneGenie.Geocoder.Models.Geo;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class BingGeocoder : IGeocoder
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
                var content = JsonConvert.DeserializeObject<Response>(json);

                var statusCode = ValidateResponse(content);
                if (statusCode != GeocodeStatus.Success)
                {
                    return new GeocodeResponseDto(statusCode);
                }

                var status = ExtractStatus(content.StatusDescription);
                if (status.IsPermanentError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned permanent error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, content.StatusDescription);
                    return new GeocodeResponseDto(GeocodeStatus.Error);
                }

                if (status.IsTemporaryError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned temporary error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, content.StatusDescription);
                    return new GeocodeResponseDto(GeocodeStatus.TemporaryError);
                }

                logger.LogTrace((int)LogEventIds.Success, "Geocode completed successfully for {address}.", geocodeRequest.Address);
                return new GeocodeResponseDto(GeocodeStatus.Success)
                {
                    Locations = content
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
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.GeocodeException, ex, "Call to geocoder failed for {address}", geocodeRequest.Address);
                return new GeocodeResponseDto(GeocodeStatus.Error);
            }
        }

        public GeocoderStatusMapping ExtractStatus(string statusText)
        {
            if (string.IsNullOrWhiteSpace(statusText))
            {
                return BingStatusMappings.First(s => s.Status == GeocodeStatus.Error);
            }

            var statusRow = BingStatusMappings.FirstOrDefault(s => s.StatusText == statusText.Trim());
            if (statusRow == null)
            {
                return BingStatusMappings.First(s => s.Status == GeocodeStatus.Error);
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

            // Bing sends back this header instead of HTTP status 429 (back-off) when too many requests.
            if (response.Headers.Any(h => h.Key == "X-MS-BM-WS-INFO" && h.Value.Any(v => v == "1")))
            {
                // TODO: Need to tie the address being backed off from in via some kind of correlation ID.
                logger.LogWarning((int)LogEventIds.GeocoderTooManyRequests, "Backoff received from geocoder");
                return GeocodeStatus.TooManyRequests;
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

        private GeocodeStatus ValidateResponse(Response content)
        {
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
