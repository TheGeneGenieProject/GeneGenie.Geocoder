// <copyright file="FakeTimeProvider.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    /// <summary>
    /// Wrapper around DateTimeOffset which can be changed for unit testing.
    /// </summary>
    public class FakeTimeProvider : ITimeProvider
    {
        private DateTimeOffset? dateTimeNow;

        /// <summary> Normally this is a pass-through to DateTimeOffset.UtcNow, but it can be overridden with SetDateTime( .. ) for testing or debugging.
        /// </summary>
        public DateTimeOffset UtcNow()
        {
            if (dateTimeNow != null)
            {
                return dateTimeNow.Value;
            }

            return DateTimeOffset.UtcNow;
        }

        /// <summary> Set time to return when SystemDateTimeOffset.UtcNow() is called.
        /// </summary>
        public void SetDateTime(DateTimeOffset dateTimeNow)
        {
            this.dateTimeNow = dateTimeNow;
        }

        /// <summary> Resets SystemDateTimeOffset.UtcNow() to return DateTimeOffset.Now.
        /// </summary>
        public void ResetDateTime()
        {
            this.dateTimeNow = null;
        }
    }
}
