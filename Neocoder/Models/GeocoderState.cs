// <copyright file="GeocoderState.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Models
{
    using System;
    using Neocoder.Services;

    public class GeocoderState
    {
        public DateTimeOffset DoNotUseBefore { get; set; }

        public GeocoderNames GeocoderId { get; set; }

        public TimeSpan NextDelay { get; set; }

        public int RequestCount { get; set; }
    }
}
