// <copyright file="ResolutionTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.Geo
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Neocoder.ExtensionMethods;
    using Neocoder.Interfaces;
    using Neocoder.Models;
    using Neocoder.Services;
    using Neocoder.UnitTests.Setup;
    using Xunit;

    /// <summary>
    /// Tests to ensure that fake and concrete classes have been registered for the geocoders and can all be resolved.
    /// </summary>
    public class ResolutionTests
    {
        [Fact]
        public void Google_geocoder_can_be_resolved()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();

            Assert.Equal(typeof(GoogleGeocoder), geocoder.GetType());
        }

        [Fact]
        public void Bing_concrete_geocoder_can_be_resolved()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();

            Assert.Equal(typeof(BingGeocoder), geocoder.GetType());
        }

        [Fact]
        public void Google_concrete_geocoder_is_resolved_by_interface_when_not_using_test_injection()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };
            var sp = new ServiceCollection()
                .AddSingleton(geocoderSettings)
                .AddGeocoders()
                .BuildServiceProvider();

            var geocoders = sp.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GetType() == typeof(GoogleGeocoder));
        }

        [Fact]
        public void Bing_concrete_geocoder_is_resolved_by_interface_when_not_using_test_injection()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Bing },
            };
            var sp = new ServiceCollection()
                .AddSingleton(geocoderSettings)
                .AddGeocoders()
                .BuildServiceProvider();

            var geocoders = sp.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GetType() == typeof(BingGeocoder));
        }

        [Fact]
        public void Google_fake_geocoder_is_registered_against_interface()
        {
            var geocoders = ConfigureDi.Services.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GeocoderId == GeocoderNames.Google);
        }

        [Fact]
        public void Bing_fake_geocoder_is_registered_against_interface()
        {
            var geocoders = ConfigureDi.Services.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GeocoderId == GeocoderNames.Bing);
        }

        /// <summary>
        /// Test to flag that a new geocoder has been registered in code so that
        /// additional tests for that geocoder can be added above and in other test classes.
        /// </summary>
        [Fact]
        public void Only_known_geocoders_have_been_registered_against_interface()
        {
            var geocoders = ConfigureDi.Services.GetServices<IGeocoder>().ToList();

            geocoders.Remove(geocoders.Single(g => g.GeocoderId == GeocoderNames.Google));
            geocoders.Remove(geocoders.Single(g => g.GeocoderId == GeocoderNames.Bing));

            Assert.Empty(geocoders);
        }
    }
}
