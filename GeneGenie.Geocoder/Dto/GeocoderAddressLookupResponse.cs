// <copyright file="GeocoderAddressLookupResponse.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    /// <summary>
    /// Holds the response from the common address lookup code when passing it back to the
    /// geocoder for further processing.
    /// </summary>
    /// <typeparam name="T">The C# type equivalent of the JSON response, unique to each external API.</typeparam>
    internal class GeocoderAddressLookupResponse<T>
    {
        internal GeocoderAddressLookupResponse(ResponseDetail responseDetail)
        {
            this.ResponseDetail = responseDetail;
        }

        /// <summary>
        /// The C# type equivalent of the JSON response, unique to each external API.
        /// </summary>
        internal T Content { get; set; }

        /// <summary>
        /// The status of the geocode lookup.
        /// </summary>
        internal ResponseDetail ResponseDetail { get; set; }
    }
}
