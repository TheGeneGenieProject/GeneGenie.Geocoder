// <copyright file="GeocodeManager.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder
{
    /// <summary>
    /// The main entry point for looking up an address.
    /// Handles selection of the next available geocoder and retrying other geocoders if the response is not usable.
    /// </summary>
    public class GeocodeManager : IGeocodeManager
    {
        private readonly IGeocoderSelector geocoderSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeManager"/> class.
        /// </summary>
        /// <param name="geocoderSelector">An instance of <see cref="IGeocoderSelector"/> that is responsible for returning the next available geocoder.</param>
        public GeocodeManager(IGeocoderSelector geocoderSelector)
        {
            this.geocoderSelector = geocoderSelector;
        }

        /// <summary>
        /// Creates an instance of <see cref="GeocodeManager"/> and if this is the first time called initialises the library with the passed
        /// geocoder settings.
        /// </summary>
        /// <param name="geocoderSettings">A list of API keys for the different geocoder services.</param>
        /// <returns>An instance of <see cref="GeocodeManager"/> which can be used to run an address geocode.</returns>
        public static GeocodeManager Create(List<GeocoderSettings> geocoderSettings)
        {
            if (geocoderSettings == null)
            {
                return null;
            }

            var serviceProvider = new ServiceCollection()
                .AddGeocoders(geocoderSettings)
                .BuildServiceProvider();
            return serviceProvider.GetRequiredService<GeocodeManager>();
        }

        /// <summary>
        /// Takes an address as a string and then selects the next available geocoder to
        /// look up the address. Cycles through geocoders if they fails in order to get a result.
        /// </summary>
        /// <param name="address">The plain text address to look up.</param>
        /// <returns>A <see cref="GeocodeResponse"/> with the status of the lookup and a list of locations found.</returns>
        public async Task<GeocodeResponse> GeocodeAddressAsync(string address)
        {
            var geocodeRequest = new GeocodeRequest
            {
                Address = address,
                BoundsHint = null,
            };
            var addressLookupResult = new GeocodeResponse();

            var geocodersTried = new Dictionary<GeocoderNames, GeocodeStatus>();
            do
            {
                var excludeGeocoders = geocodersTried.Select(g => g.Key).ToList();
                var geocoder = await geocoderSelector.SelectNextGeocoderAsync(excludeGeocoders);
                if (geocoder == null)
                {
                    break;
                }

                var geocoderResponse = await geocoder.GeocodeAddressAsync(geocodeRequest);

                if (geocoderResponse.ResponseStatus == GeocodeStatus.Success)
                {
                    addressLookupResult.Locations = geocoderResponse
                        .Locations
                        .Select(l => new GeocodeResponseLocation
                        {
                            Bounds = l.Bounds,
                            FormattedAddress = l.FormattedAddress,
                            Location = l.Location,
                        })
                        .ToList();
                    addressLookupResult.GeocoderId = geocoder.GeocoderId;
                    addressLookupResult.Status = AddressLookupStatus.Geocoded;
                }

                geocodersTried.Add(geocoder.GeocoderId, geocoderResponse.ResponseStatus);
            }
            while (addressLookupResult.Status != AddressLookupStatus.Geocoded);

            addressLookupResult.Status = SummariseGeocodeStatus(geocodersTried);
            return addressLookupResult;
        }

        private AddressLookupStatus SummariseGeocodeStatus(Dictionary<GeocoderNames, GeocodeStatus> geocodersTried)
        {
            if (geocodersTried.ContainsValue(GeocodeStatus.Success))
            {
                return AddressLookupStatus.Geocoded;
            }

            if (geocodersTried.All(g => g.Value == GeocodeStatus.ZeroResults))
            {
                return AddressLookupStatus.ZeroResults;
            }

            if (geocodersTried.All(g => g.Value == GeocodeStatus.Error || g.Value == GeocodeStatus.InvalidRequest))
            {
                return AddressLookupStatus.PermanentGeocodeError;
            }

            if (geocodersTried.Any(g => g.Value == GeocodeStatus.RequestDenied || g.Value == GeocodeStatus.TooManyRequests))
            {
                return AddressLookupStatus.TemporaryGeocodeError;
            }

            return AddressLookupStatus.MultipleIssues;
        }
    }
}
