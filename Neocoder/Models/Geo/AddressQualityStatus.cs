// <copyright file="AddressQualityStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Models.Geo
{
    /// <summary>
    /// Addresses are parsed to one of these status' before doing an expensive
    /// lookup to see if they are junk data or not.
    /// </summary>
    public enum AddressQualityStatus
    {
        NotSet = 0,
        SeemsToBeADate = 1,
        AllNumeric = 2,
        KnownErroneous = 3,
        Empty = 4,
        OK = 100,
    }
}
