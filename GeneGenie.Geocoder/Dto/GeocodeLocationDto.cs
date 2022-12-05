﻿// <copyright file="GeocodeLocationDto.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Dto
{
    public class GeocodeLocationDto
    {
        public Bounds Bounds { get; set; }

        public string FormattedAddress { get; set; }

        public LocationPair Location { get; set; }
    }
}
