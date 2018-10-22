// <copyright file="DateFormat.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Models
{
    public enum DateFormat
    {
        /// <summary>This date has not been parsed yet.</summary>
        NotSet = 0,

        Dd_mm_yyyy = 1,

        Mm_dd_yyyy = 2,

        Yyyy_mm_dd = 3,

        Yyyy_dd_mm = 4,

        /// <summary>
        /// Could be either <see cref="Dd_mm_yyyy"/> or <see cref="Mm_dd_yyyy"/> depending on the
        /// surrounding records.
        /// </summary>
        UnsureStartingWithDateOrMonth = 5,

        /// <summary>
        /// Could be either <see cref="Yyyy_mm_dd"/> or <see cref="Yyyy_dd_mm"/> depending on the
        /// surrounding records.
        /// </summary>
        UnsureEndingWithDateOrMonth = 5,

        Yyyy = 6,
        Yyyy_mm = 7,
        Mm_yyyy = 8,

        UnableToParse = 9,
        Mmm = 10,
        Mmm_dd = 11,
        Dd_mmm = 12,
        Yyyy_mmm = 13,
        Mmm_yyyy = 14,
        Yyyy_mmm_dd = 15,
        Yyyy_dd_mmm = 16,
        Mmm_dd_yyyy = 17,
        Dd_mmm_yyyy = 18,
        Mm = 19,
        Dd_mm = 20,
        Mm_dd = 21,

        UnableToParseAsYearInMiddle = 22,
    }
}
