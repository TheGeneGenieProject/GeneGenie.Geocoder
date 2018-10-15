// <copyright file="DocumentDbGeocoderSelector.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Services.Selectors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Neocoder.Interfaces;

    internal class DocumentDbGeocoderSelector : IGeocoderSelector
    {
        public Task<IGeocoder> SelectNextGeocoderAsync()
        {
            throw new System.NotImplementedException("This is where we would call a DocumentDB/CosmosDB sp and request the next geocoder atomically.");
        }

        public Task<IGeocoder> SelectNextGeocoderAsync(List<GeocoderNames> excludeGeocoders)
        {
            throw new System.NotImplementedException("This is where we would call a DocumentDB/CosmosDB sp and request the next geocoder atomically.");
        }
    }
}
