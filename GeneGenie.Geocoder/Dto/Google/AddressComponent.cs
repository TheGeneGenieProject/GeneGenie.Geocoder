// <copyright file="AddressComponent.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// A component of the address returned such as 'street number'.
    /// 
    /// For more detail, see https://developers.google.com/maps/documentation/geocoding/requests-geocoding#Types
    /// </summary>
    public class AddressComponent
    {
        /// <summary>
        /// The full value of this part of the address. For example 'Helsinki'.
        /// </summary>
        public string Long_name { get; set; }

        /// <summary>
        /// An abbreviated version of <see cref="Long_name"/>, for example 'HKI' for Helsinki.
        /// </summary>
        public string Short_name { get; set; }

        /// <summary>
        /// The types defining the part of the address that this instance holds.
        /// </summary>
        public List<string> Types { get; set; }
    }
}
