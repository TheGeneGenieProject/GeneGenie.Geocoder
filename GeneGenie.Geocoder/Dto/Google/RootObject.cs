// <copyright file="RootObject.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Google
{
    /// <summary>
    /// The root of an address lookup response from the Google API.
    /// </summary>
    public class RootObject
    {
        public string Error_message { get; set; }

        public bool Partial_match { get; set; }

        public List<Result> Results { get; set; }

        public string Status { get; set; }
    }
}
