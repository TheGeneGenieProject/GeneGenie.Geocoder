// <copyright file="IGeocoderSelector.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// A instance of <see cref="IGeocoderSelector"/> is used to request the next available geocoder
    /// (<see cref="IGeocoder"/>).
    /// For a <see cref="IGeocoderSelector"/> to work properly it must be aware of previous geocoder
    /// selections, hence it requires storage. This interface enables the plugging in of
    /// different selectors that use different storage (database or memory).
    /// For example, local testing and single process apps can use <see cref="Geo.Selectors.InMemoryGeocoderSelector"/>
    /// whilst multi-instance cloud based functions use <see cref="Geo.Selectors.DocumentDbGeocoderSelector"/>
    /// which uses a stored procedure in the database to make the selector decision.
    /// </summary>
    public interface IGeocoderSelector
    {
        /// <summary>
        /// Select the next available geocoder.
        /// </summary>
        /// <returns></returns>
        Task<IGeocoder> SelectNextGeocoderAsync();

        /// <summary>
        /// Select the next available geocoder, avoiding the passed list of geocoders (they may have already
        /// been tried and failed to return a result).
        /// </summary>
        /// <param name="excludeGeocoders"></param>
        /// <returns></returns>
        Task<IGeocoder> SelectNextGeocoderAsync(List<GeocoderNames> excludeGeocoders);
    }
}
