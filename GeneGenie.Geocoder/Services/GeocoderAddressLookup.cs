﻿// <copyright file="GeocoderAddressLookup.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    using GeneGenie.Geocoder.Dto;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Models.Geo;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains code that is common to geocoders for making the call to the external API
    /// and handling the response.
    /// </summary>
    /// <typeparam name="T">The C# type equivalent of response we are expect to parse out from the received JSON.</typeparam>
    internal class GeocoderAddressLookup<T>
    {
        private readonly IGeocoderAddressProcessor<T> geocoderAddressProcessor;
        private readonly ILogger logger;

        internal GeocoderAddressLookup(IGeocoderAddressProcessor<T> geocoderAddressProcessor, ILogger logger)
        {
            this.geocoderAddressProcessor = geocoderAddressProcessor;
            this.logger = logger;
        }

        /// <summary>
        /// Given an address will check it for validity, call the API and parse the most common error states.
        /// </summary>
        /// <param name="geocodeRequest">The address to lookup.</param>
        /// <returns>The status of the call, if OK then the content can be parsed for the results.</returns>
        internal async Task<GeocoderAddressLookupResponse<T>> GeocodeAddressAsync(GeocodeRequest geocodeRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(geocodeRequest.Address))
                {
                    logger.LogWarning((int)LogEventIds.GeocoderInputEmpty, "Data passed to geocoder was empty.");
                    return new GeocoderAddressLookupResponse<T>(GeocodeStatus.InvalidRequest);
                }

                var response = await geocoderAddressProcessor.MakeApiRequestAsync(geocodeRequest);

                var httpStatus = geocoderAddressProcessor.ValidateHttpResponse(response);
                if (httpStatus != GeocodeStatus.Success)
                {
                    return new GeocoderAddressLookupResponse<T>(httpStatus);
                }

                var json = await response.Content.ReadAsStringAsync();
                logger.LogTrace((int)LogEventIds.GeocoderResponse, json);
                var content = JsonConvert.DeserializeObject<T>(json);

                var statusCode = geocoderAddressProcessor.ValidateResponse(content);
                if (statusCode != GeocodeStatus.Success)
                {
                    return new GeocoderAddressLookupResponse<T>(statusCode);
                }

                var status = geocoderAddressProcessor.ExtractStatus(content);
                if (status.IsPermanentError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned permanent error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, status.StatusText);
                    return new GeocoderAddressLookupResponse<T>(GeocodeStatus.Error);
                }

                if (status.IsTemporaryError)
                {
                    logger.LogWarning((int)LogEventIds.GeocoderError, "Geocoder returned temporary error for {address} with status of {status} and error detail of {error}", geocodeRequest.Address, status.Status, status.StatusText);
                    return new GeocoderAddressLookupResponse<T>(GeocodeStatus.TemporaryError);
                }

                logger.LogTrace((int)LogEventIds.Success, "Geocode completed successfully for {address}.", geocodeRequest.Address);
                return new GeocoderAddressLookupResponse<T>(GeocodeStatus.Success)
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.GeocodeException, ex, "Call to geocoder failed for {address}", geocodeRequest.Address);
                return new GeocoderAddressLookupResponse<T>(GeocodeStatus.Error);
            }
        }
    }
}