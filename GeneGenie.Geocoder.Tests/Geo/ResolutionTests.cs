// <copyright file="ResolutionTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Geo
{
    /// <summary>
    /// Tests to ensure that fake and concrete classes have been registered for the geocoders and can all be resolved.
    /// </summary>
    public class ResolutionTests
    {
        /// <summary>
        /// Checks the Google geocoder service can be resolved without error.
        /// </summary>
        [Fact]
        public void Google_concrete_geocoder_can_be_resolved()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();

            Assert.Equal(typeof(GoogleGeocoder), geocoder.GetType());
        }

        /// <summary>
        /// Checks the Google geocoder has the correct id set.
        /// </summary>
        [Fact]
        public void Google_concrete_geocoder_has_correct_id()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();

            Assert.Equal(GeocoderNames.Google, geocoder.GeocoderId);
        }

        /// <summary>
        /// Checks the Bing geocoder service can be resolved without error.
        /// </summary>
        [Fact]
        public void Bing_concrete_geocoder_can_be_resolved()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();

            Assert.Equal(typeof(BingGeocoder), geocoder.GetType());
        }

        /// <summary>
        /// Checks the Bing geocoder has the correct id set.
        /// </summary>
        [Fact]
        public void Bing_concrete_geocoder_has_correct_id()
        {
            var geocoder = ConfigureDi.Services.GetRequiredService<BingGeocoder>();

            Assert.Equal(GeocoderNames.Bing, geocoder.GeocoderId);
        }

        /// <summary>
        /// Checks the Google geocoder service can be resolved by interface definition.
        /// </summary>
        [Fact]
        public void Google_concrete_geocoder_is_resolved_by_interface_when_not_using_test_injection()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Google },
            };
            var sp = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .BuildServiceProvider();

            var geocoders = sp.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GetType() == typeof(GoogleGeocoder));
        }

        /// <summary>
        /// Checks the Bing geocoder service can be resolved by interface definition.
        /// </summary>
        [Fact]
        public void Bing_concrete_geocoder_is_resolved_by_interface_when_not_using_test_injection()
        {
            var geocoderSettings = new List<GeocoderSettings>
            {
                new GeocoderSettings { GeocoderName = GeocoderNames.Bing },
            };
            var sp = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .BuildServiceProvider();

            var geocoders = sp.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GetType() == typeof(BingGeocoder));
        }

        /// <summary>
        /// Checks that our fake unit testable Google geocoder service can be resolved by interface definition.
        /// </summary>
        [Fact]
        public void Google_fake_geocoder_is_registered_against_interface()
        {
            var geocoders = ConfigureDi.Services.GetServices<IGeocoder>();

            Assert.Contains(geocoders, g => g.GeocoderId == GeocoderNames.Google);
        }

        /// <summary>
        /// Checks that our fake unit testable Bing geocoder service can be resolved by interface definition.
        /// </summary>
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
