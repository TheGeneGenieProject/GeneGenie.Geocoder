// <copyright file="IGeocoderAddressProcessor.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// Common address validation, parsing and API calls that are used across multiple Geocoders.
    /// The geocoder implements this interface so that it can be called from geocoder agnostic code
    /// such as making the address geocoding call to the external service via REST.
    /// 
    /// Google and Bing geocoders use this approach, other new geocoders may not which is why this is a
    /// separate interface to <see cref="IGeocoder"/>. If future geocoders also use the same
    /// approach then we'll bundle this into the main interface.
    /// </summary>
    /// <typeparam name="T">The C# equivalent of the JSON response received from the API call.</typeparam>
    public interface IGeocoderAddressProcessor<T>
    {
        /// <summary>
        /// The geocoder implements this to extract the status of the API call from the response.
        /// </summary>
        /// <param name="content">The C# type received from the API call, obtained by conterting the JSON response.</param>
        /// <returns>A type defining the status of the call.</returns>
        GeocoderStatusMapping ExtractStatus(T content);

        /// <summary>
        /// The geocoder implements this to construct the request and make the API call.
        /// </summary>
        /// <param name="geocodeRequest">The address to lookup.</param>
        /// <returns>A HTTP response that will be parsed by <see cref="GeocoderAddressLookup{T}"/>.</returns>
        Task<HttpResponseMessage> MakeApiRequestAsync(GeocodeRequest geocodeRequest);

        /// <summary>
        /// Validates the HTTP status code of the response which can be different for each geocoder service.
        /// </summary>
        /// <param name="response">The HTTP level response received from the API call.</param>
        /// <returns>A code reflecting the network level status of the API call.</returns>
        GeocodeStatus ValidateHttpResponse(HttpResponseMessage response);

        /// <summary>
        /// Validates the HTTP content of the response which can be different for each geocoder service.
        /// </summary>
        /// <param name="content">An instance of the C# equivalent of the JSON response received from the API call.</param>
        /// <returns>A code reflecting the content status of the API call.</returns>
        GeocodeStatus ValidateResponse(T content);
    }
}
