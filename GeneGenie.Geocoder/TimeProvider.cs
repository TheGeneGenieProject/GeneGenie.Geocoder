// <copyright file="TimeProvider.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder
{
    using System;
    using GeneGenie.Geocoder.Interfaces;

    /// <summary>
    /// Implementation of <see cref="ITimeProvider"/> for live system use.
    /// </summary>
    public class TimeProvider : ITimeProvider
    {
        /// <summary>A pass-through to <see cref="DateTimeOffset.UtcNow"/>.</summary>
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
