// <copyright file="FakeGeocoderBase.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    /// <summary>
    /// Common base class for fake geocoders.
    /// Only used by unit tests.
    /// </summary>
    public abstract class FakeGeocoderBase
    {
        /// <summary>
        /// The name of the geocoder we are faking.
        /// </summary>
        public abstract GeocoderNames GeocoderId { get; }

        internal static Dictionary<GeocoderNames, ResponseDetail> ExtractResponseDetailFromAddress(string address)
        {
            var statusList = new Dictionary<GeocoderNames, ResponseDetail>();
            var splitByGeocoder = address.Split(";");

            foreach (var statusPair in splitByGeocoder)
            {
                var splitByNameAndStatus = statusPair.Split("=");

                var geocoderId = Enum.Parse<GeocoderNames>(splitByNameAndStatus[0], true);
                var geocoderStatus = Enum.Parse<GeocodeStatus>(splitByNameAndStatus[1], true);
                var responseDetail = new ResponseDetail("XX", geocoderStatus);

                statusList.Add(geocoderId, responseDetail);
            }

            return statusList;
        }
    }
}
