// <copyright file="ITimeProvider.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Interfaces
{
    public interface ITimeProvider
    {
        DateTimeOffset UtcNow();
    }
}
