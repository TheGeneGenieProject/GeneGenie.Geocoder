// <copyright file="ResourceSet.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Dto.Bing
{
    using System.Collections.Generic;

    public class ResourceSet
    {
        public long EstimatedTotal { get; set; }

        public List<Resource> Resources { get; set; }
    }
}
