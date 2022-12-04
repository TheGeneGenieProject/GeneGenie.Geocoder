// <copyright file="InstantiationTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocodeManagerTests
{
    /// <summary>
    /// Tests to check that the library can 'self register' and returns an instance of a <see cref="GeocodeManager"/>.
    /// </summary>
    public class InstantiationTests
    {
        [Fact]
        public void Self_registration_does_not_succeed_with_null_list()
        {
            var geocodeManager = GeocodeManager.Create(null);

            Assert.Null(geocodeManager);
        }

        [Fact]
        public void Self_registration_succeeds_with_empty_list()
        {
            var geocoderSettings = new List<GeocoderSettings>();

            var geocodeManager = GeocodeManager.Create(geocoderSettings);

            Assert.NotNull(geocodeManager);
        }
    }
}
