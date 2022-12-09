// <copyright file="GeocoderUrlHelper.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Services
{
    internal static class GeocoderUrlHelper
    {
        /// <summary>
        /// Bing and Google use a very similar URL format apart from the parameter names. This code
        /// services both geocoders and enables them to pass in their own parameter names.
        /// </summary>
        /// <param name="geocodeRequest">The address lookup where we find the parameter values.</param>
        /// <param name="parameters">The parameter collection to add to.</param>
        /// <param name="localeKey">The name of the locale/language parameter.</param>
        /// <param name="regionKey">The name of the region parameter.</param>
        /// <param name="boundsKey">The name of the bounding area parameter.</param>
        internal static void AddUrlParameters(GeocodeRequest geocodeRequest, Dictionary<string, string> parameters, string localeKey, string regionKey, string boundsKey)
        {
            if (!string.IsNullOrWhiteSpace(geocodeRequest.Locale))
            {
                parameters.Add(localeKey, geocodeRequest.Locale);
            }

            if (!string.IsNullOrWhiteSpace(geocodeRequest.Region))
            {
                parameters.Add(regionKey, geocodeRequest.Region);
            }

            if (geocodeRequest.BoundsHint != null)
            {
                var sw = $"{geocodeRequest.BoundsHint.SouthWest.Latitude},{geocodeRequest.BoundsHint.SouthWest.Longitude}";
                var ne = $"{geocodeRequest.BoundsHint.NorthEast.Latitude},{geocodeRequest.BoundsHint.NorthEast.Longitude}";
                parameters.Add(boundsKey, $"{sw},{ne}");
            }
        }
    }
}
