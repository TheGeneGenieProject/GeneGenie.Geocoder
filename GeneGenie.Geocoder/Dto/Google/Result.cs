// <copyright file="Result.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    public class Result
    {
        public List<AddressComponent> Address_components { get; set; }

        public string Formatted_address { get; set; }

        public Geometry Geometry { get; set; }

        public string Place_id { get; set; }

        public List<string> Types { get; set; }
    }
}
