// <copyright file="Response.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    public class Response
    {
        public string AuthenticationResultCode { get; set; }

        public List<string> ErrorDetails { get; set; }

        public List<ResourceSet> ResourceSets { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string TraceId { get; set; }
    }
}
