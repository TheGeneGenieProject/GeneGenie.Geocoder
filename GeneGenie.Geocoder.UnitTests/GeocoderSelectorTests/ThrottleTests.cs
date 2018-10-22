// <copyright file="ThrottleTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.UnitTests.GeocoderSelectorTests
{
    using System;
    using System.Threading.Tasks;
    using GeneGenie.Geocoder.Interfaces;
    using GeneGenie.Geocoder.Services;
    using GeneGenie.Geocoder.Services.Selectors;
    using GeneGenie.Geocoder.UnitTests.Fakes;
    using GeneGenie.Geocoder.UnitTests.Setup;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ThrottleTests
    {
        private readonly InMemoryGeocoderSelector geocoderSelector;
        private readonly ITimeProvider timeProvider;

        public ThrottleTests()
        {
            var sp = ConfigureDi.BuildDi(true);
            geocoderSelector = sp.GetRequiredService<InMemoryGeocoderSelector>();
            timeProvider = sp.GetRequiredService<ITimeProvider>();
        }

        [Fact]
        public async Task No_geocoders_are_returned_when_they_are_all_set_as_throttled()
        {
            var date = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Bing).DoNotUseBefore = date.AddDays(1);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Google).DoNotUseBefore = date.AddDays(1);
            ((FakeTimeProvider)timeProvider).SetDateTime(date);

            var geocoder = await geocoderSelector.SelectNextGeocoderAsync();

            Assert.Null(geocoder);
        }

        [Fact]
        public async Task Bing_is_repeatedly_resolved_when_google_is_set_as_throttled()
        {
            var date = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Bing).DoNotUseBefore = date.AddDays(-1);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Google).DoNotUseBefore = date.AddDays(1);
            ((FakeTimeProvider)timeProvider).SetDateTime(date);

            Assert.Equal(GeocoderNames.Bing, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
            Assert.Equal(GeocoderNames.Bing, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
        }

        [Fact]
        public async Task Google_is_repeatedly_resolved_when_bing_is_marked_as_failing()
        {
            var date = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Bing).DoNotUseBefore = date.AddDays(1);
            geocoderSelector.GeocoderStatusById(GeocoderNames.Google).DoNotUseBefore = date.AddDays(-1);
            ((FakeTimeProvider)timeProvider).SetDateTime(date);

            Assert.Equal(GeocoderNames.Google, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
            Assert.Equal(GeocoderNames.Google, (await geocoderSelector.SelectNextGeocoderAsync()).GeocoderId);
        }
    }
}
