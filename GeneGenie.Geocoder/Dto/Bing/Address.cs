// <copyright file="Address.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    /// <summary>
    /// An address result as returned by Bing. A lot of these properties can be null depending on how accurate the result it.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The first line of an address.
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        /// Bing places 'England' at this level but does not document the rules for populating this.
        /// </summary>
        public string AdminDistrict { get; set; }

        /// <summary>
        /// Bing places 'London' at this level but does not document the rules for populating this.
        /// </summary>
        public string AdminDistrict2 { get; set; }

        /// <summary>
        /// This is really the country, not a region. Badly named by Bing.
        /// You'll find data such as 'United Kingdom' here.
        /// </summary>
        public string CountryRegion { get; set; }

        /// <summary>
        /// The two character ISO country code.
        /// </summary>
        public string CountryRegionIso2 { get; set; }

        /// <summary>
        /// The address for display to an end user. An example is 'Tower of London, United Kingdom'.
        /// </summary>
        public string FormattedAddress { get; set; }

        /// <summary>
        /// Seems to hold the town, city (village or hamlet?). An example of data seen here is 'London'.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// The full postal code for the address.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// For UK addresses this is the left hand side of the postal code, for example 'EC3'.
        /// </summary>
        public string Neighborhood { get; set; }

        /// <summary>
        /// The name of a landmark, for example 'Eiffel Tower' or 'Tower of London'.
        /// </summary>
        public string Landmark { get; set; }
    }
}
