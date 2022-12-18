// <copyright file="UrlFormatTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.GeocoderTests.Google
{
    /// <summary>
    /// Checks that a URL can be built for an API call.
    /// </summary>
    public class UrlFormatTests
    {
        private readonly GoogleGeocoder geocoder;

        /// <summary>
        /// Sets up test dependencies. Called by xunit only.
        /// </summary>
        public UrlFormatTests()
        {
            geocoder = ConfigureDi.Services.GetRequiredService<GoogleGeocoder>();
        }

        /// <summary>
        /// Checks that the passed address is encoded properly for a Google search.
        /// </summary>
        /// <param name="address">The address to test.</param>
        /// <param name="expectedQuerystring">What the querystring should look like after building.</param>
        [Theory]
        [InlineData("", "address=&key=&sensor=false")]
        [InlineData("toSearch", "address=toSearch&key=&sensor=false")]
        [InlineData("spacey name", "address=spacey%20name&key=&sensor=false")]
        public void Address_query_can_be_built(string address, string expectedQuerystring)
        {
            var geocodeRequest = new GeocodeRequest { Address = address };

            var url = geocoder.BuildUrl(geocodeRequest);

            Assert.EndsWith(expectedQuerystring, url);
        }

        /// <summary>
        /// Checks that the passed locale is encoded properly for a Google search.
        /// </summary>
        /// <param name="locale">The locale to test.</param>
        /// <param name="expectedQuerystring">What the querystring should look like after building.</param>
        [Theory]
        [InlineData("", "address=&key=&sensor=false")]
        [InlineData("fr", "&language=fr")]
        [InlineData("en-GB", "&language=en-GB")]
        public void Locale_can_be_passed(string locale, string expectedQuerystring)
        {
            var geocodeRequest = new GeocodeRequest { Locale = locale };

            var url = geocoder.BuildUrl(geocodeRequest);

            Assert.EndsWith(expectedQuerystring, url);
        }

        /// <summary>
        /// Checks that the passed region hint is encoded properly for a Google search.
        /// </summary>
        /// <param name="region">The region to test.</param>
        /// <param name="expectedQuerystring">What the querystring should look like after building.</param>
        [Theory]
        [InlineData("", "address=&key=&sensor=false")]
        [InlineData("fr", "&region=fr")]
        [InlineData("uk", "&region=uk")]
        public void Region_can_be_passed(string region, string expectedQuerystring)
        {
            var geocodeRequest = new GeocodeRequest { Region = region };

            var url = geocoder.BuildUrl(geocodeRequest);

            Assert.EndsWith(expectedQuerystring, url);
        }

        /// <summary>
        /// Checks that the passed bounds are encoded properly for a Google search.
        /// </summary>
        /// <param name="n">The northern most bound (latitude).</param>
        /// <param name="e">The eastern most bound (longitude).</param>
        /// <param name="s">The southern most bound (latitude).</param>
        /// <param name="w">The western most bound (longitude).</param>
        /// <param name="expectedQuerystring">What the querystring should look like after building.</param>
        [Theory]
        [InlineData(3, 4, 1, 2, "&bounds=1,2%7C3,4")]
        [InlineData(3.3333, 4.4444, 1.1111, 2.2222, "&bounds=1.1111,2.2222%7C3.3333,4.4444")]
        public void Bounds_can_be_passed(double n, double e, double s, double w, string expectedQuerystring)
        {
            var geocodeRequest = new GeocodeRequest
            {
                BoundsHint = new Bounds
                {
                    NorthEast = new LocationPair { Latitude = n, Longitude = e },
                    SouthWest = new LocationPair { Latitude = s, Longitude = w },
                }
            };

            var url = geocoder.BuildUrl(geocodeRequest);

            Assert.EndsWith(expectedQuerystring, url);
        }
    }
}
