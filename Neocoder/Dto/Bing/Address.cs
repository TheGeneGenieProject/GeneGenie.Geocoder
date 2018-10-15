// <copyright file="Address.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Dto.Bing
{
    public class Address
    {
        public string AddressLine { get; set; }

        public string AdminDistrict { get; set; }

        public string AdminDistrict2 { get; set; }

        public string CountryRegion { get; set; }

        public string CountryRegionIso2 { get; set; }

        public string FormattedAddress { get; set; }

        public string Locality { get; set; }

        public string PostalCode { get; set; }

        public string Neighborhood { get; set; }

        public string Landmark { get; set; }
    }
}
