# GeneGenie.Geocoder
A .Net geocoder library that can query multiple backend geocoders and rotate between them (briefly named Neocoder, GeneGenie.Geocoder is the new name for the project). **Currently no nuget releases available as being updated with breaking changes.**

Currently supports Google and Bing geocoder APIs but more can be added if needed (can even be added from outside of the library).

## Build status
[![Build and run tests](https://github.com/TheGeneGenieProject/GeneGenie.Geocoder/actions/workflows/sonar.yml/badge.svg)](https://github.com/TheGeneGenieProject/GeneGenie.Geocoder/actions/workflows/sonar.yml)

## Quickstart

The library can integrate with your chosen Dependency Injection framework or if you want to keep it very simple, just use the Create method as below. See the GeneGenie.Geocoder.Console project for more details on the following approaches.

### Simple usage (without Dependency Injection)

```cs
// Define the API keys for the geocoders we'll be using (should come out of your configuration file).
var geocoderSettings = new List<GeocoderSettings>
{
    new GeocoderSettings { ApiKey = "Your Bing API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Bing },
    new GeocoderSettings { ApiKey = "Your Google API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Google },
};

// Instead of doing 'var g = new GeocodeManager' we use a factory method which initialises the library.
var geocodeManager = GeocodeManager.Create(geocoderSettings);

// This first lookup will be handled by Bing (unless it fails to resolve the address, which will then fail over to Google).
var firstResult = await geocodeManager.GeocodeAddressAsync("10 Downing St., London, UK");
Console.WriteLine($"Result of first lookup, used {firstResult.GeocoderId}, status of {firstResult.Status} with {firstResult.Locations.Count} results.");
foreach (var foundLocation in firstResult.Locations)
{
    Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
}

// The following lookup then uses the other geocoder service.
var secondResult = await geocodeManager.GeocodeAddressAsync("The Acropolis, Greece");
Console.WriteLine($"Result of second lookup, used {secondResult.GeocoderId}, status of {secondResult.Status} with {secondResult.Locations.Count} results.");
foreach (var foundLocation in secondResult.Locations)
{
    Console.WriteLine($"Address of {foundLocation.FormattedAddress} has a location of {foundLocation.Location.Latitude} / {foundLocation.Location.Longitude}");
}
```

### Using with .Net Core dependency injection

Register the geocoder for use with .Net Core Dependency Injection via;

```cs
// Define the API keys for the geocoders we'll be using (should come out of your configuration file).
var geocoderSettings = new List<GeocoderSettings>
{
    new GeocoderSettings { ApiKey = "Your Bing API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Bing },
    new GeocoderSettings { ApiKey = "Your Google API key (best practice is to put this in a config file, not in source)", GeocoderName = Services.GeocoderNames.Google },
};

// Register the settings and Neocoder.
return new ServiceCollection()
    .AddGeocoders(geocoderSettings)
    .BuildServiceProvider();

// In your main code, normally you'd get this injected via DI.
var geocodeManager = serviceProvider.GetRequiredService<GeocodeManager>();

var geocoded = await geocodeManager.GeocodeAddressAsync(address);
```

## Running multiple instances
If using this library in a multi-process environment (such as serverless functions or a webfarm) then you'll need to implement your own Geocoder selector with the interface IGeocoderSelector. The class you implement would need to figure out what geocoder to select next based on what was used previously by accessing a shared resource (Redis, database layer etc), This would typically involve a locking fetch / update on the resource. See InMemoryGeocoderSelector.cs for ideas.

### Code quality
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=bugs)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![CodeQL](https://github.com/TheGeneGenieProject/GeneGenie.Geocoder/actions/workflows/codeql.yml/badge.svg)](https://github.com/TheGeneGenieProject/GeneGenie.Geocoder/actions/workflows/codeql.yml)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=coverage)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Geocoder)

## Contributing

We would love your help, see [Contributing.md](Contributing.md) for guidelines.
