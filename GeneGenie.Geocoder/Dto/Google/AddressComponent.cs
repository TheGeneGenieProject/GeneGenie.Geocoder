// <copyright file="AddressComponent.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    public class AddressComponent
    {
        public string Long_name { get; set; }

        public string Short_name { get; set; }

        public List<string> Types { get; set; }
    }
}
