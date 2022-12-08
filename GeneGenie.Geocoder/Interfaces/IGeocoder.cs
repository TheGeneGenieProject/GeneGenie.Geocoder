// <copyright file="IGeocoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// Interface that geocoders must implement in order to be called from the geocoder
    /// agnostic <see cref="GeocodeManager"/> class.
    /// </summary>
    public interface IGeocoder
    {
        /// <summary>
        /// The unique id of the geocoder. Used to tie up with the configuration settings
        /// for injecting the geocoder setup.
        /// </summary>
        GeocoderNames GeocoderId { get; }

        /// <summary>
        /// Geocode an address from the source text value.
        /// </summary>
        /// <param name="geocodeRequest">The address to geocode.</param>
        /// <returns>An object containing a list of possible results.</returns>
        Task<GeocodeResponseDto> GeocodeAddressAsync(GeocodeRequest geocodeRequest);
    }
}
