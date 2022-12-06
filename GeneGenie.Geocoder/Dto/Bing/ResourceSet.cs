// <copyright file="ResourceSet.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto.Bing
{
    /// <summary>
    /// Holds the resources returned by the Bing location search.
    /// </summary>
    public class ResourceSet
    {
        /// <summary>
        /// An estimate of the total number of resources in the ResourceSet.
        /// </summary>
        public long EstimatedTotal { get; set; }

        /// <summary>
        /// A collection of resources returned, which in this case are location API results.
        /// </summary>
        public List<Resource> Resources { get; set; }
    }
}
