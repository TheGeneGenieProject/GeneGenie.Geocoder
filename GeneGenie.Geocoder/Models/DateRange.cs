// <copyright file="DateRange.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models
{
    using System;

    public class DateRange
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public DateFormat SourceFormat { get; set; }

        public DateRangeScope Scope { get; set; }

        public string Source { get; set; }
    }
}
