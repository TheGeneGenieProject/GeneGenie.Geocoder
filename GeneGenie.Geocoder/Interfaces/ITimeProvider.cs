// <copyright file="ITimeProvider.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    /// <summary>
    /// A pluggable way to replace the system clock when testing.
    /// In production, will resolve to a concrete type that wraps
    /// the real system clock.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Retrieve the current system date and time. Used by unit tests to return a known time.
        /// </summary>
        /// <returns>A DateTimeOffset representing the current system time.</returns>
        DateTimeOffset UtcNow();
    }
}
