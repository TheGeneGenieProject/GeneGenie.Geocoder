// <copyright file="IGeocodeManager.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// Interface for the main usage point of the geocoder library so that it can be swapped out for testing.
    /// This resolves to <see cref="GeocodeManager"/> in normal system use and is only used by internal tests.
    /// </summary>
    public interface IGeocodeManager
    {
        /// <summary>
        /// Given an address will attempt to geocode it with the registered geocoders, failing over to the other geocoders
        /// if a lookup fails.
        /// </summary>
        /// <param name="address">The address to geocode.</param>
        /// <returns>An instance of <see cref="GeocodeResponse"/> that indicates success or fail and the locations found.</returns>
        Task<GeocodeResponse> GeocodeAddressAsync(string address);
    }
}
