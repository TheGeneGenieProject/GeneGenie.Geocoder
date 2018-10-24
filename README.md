# GeneGenie.Geocoder
A .Net standard geocoder library that can query multiple backend geocoders and rotate between them (briefly named Neocoder, GeneGenie.Geocoder is the new name for the project).

Currently supports Google and Bing geocoder APIs.

## Status
[![AppVeyor branch](https://img.shields.io/appveyor/ci/RyanONeill1970/genegenie-geocoder/master.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-geocoder) [![NuGet](https://img.shields.io/nuget/v/GeneGenie.Geocoder.svg)](https://www.nuget.org/packages/GeneGenie.Geocoder) [![AppVeyor tests](https://img.shields.io/appveyor/tests/RyanONeill1970/genegenie-geocoder.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-geocoder/build/tests)

## Quickstart

See the GeneGenie.Geocoder.Console project for an example of the following.
Set your Google and Bing geocoder API keys in appsettings.json.
Register Neocoder for use with .Net Core Dependency Injection via;

            // This gets the geocoder settings out of your configuration file.
            var appSettings = configuration.GetSection("App").Get<AppSettings>();

            // Register the settings and Neocoder.
            return new ServiceCollection()
                .AddSingleton(appSettings.GeocoderSettings)
                .AddGeocoders()
                .BuildServiceProvider();

In your main code

                // Normally you'd get this injected via DI.
                var geocodeManager = serviceProvider.GetRequiredService<GeocodeManager>();

                var geocoded = await geocodeManager.GeocodeAddressAsync(address);


## Running multiple instances
If using this library in a multi-process environment (such as serverless functions or a webfarm) then you'll need to implement your own Geocoder selector with the interface IGeocoderSelector. The class you implement would need to figure out what geocoder to select next based on what was used previously by accessing a shared resource (Redis, database layer etc), This would typically involve a locking fetch / update on the resource. See InMemoryGeocoderSelector.cs for ideas.

### Code quality
[![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder) [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=alert_status)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=bugs)](https://sonarcloud.io/component_measures?id=GeneGenie.Geocoder&metric=Reliability) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=vulnerabilities)](https://sonarcloud.io/component_measures?id=GeneGenie.Geocoder&metric=Security) [![Code smells](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=code_smells)](https://sonarcloud.io/component_measures?id=GeneGenie.Geocoder&metric=Maintainability) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=coverage)](https://sonarcloud.io/component_measures?id=GeneGenie.Geocoder&metric=Coverage) [![Duplications](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=duplicated_lines_density)](https://sonarcloud.io/component_measures?id=GeneGenie.Geocoder&metric=Duplications) [![Reliability](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder) [![Security](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=security_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder) [![Security](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=sqale_index)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder) [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Geocoder&metric=ncloc)](https://sonarcloud.io/dashboard?id=GeneGenie.Geocoder)

[![Build stats](https://buildstats.info/appveyor/chart/ryanoneill1970/genegenie-geocoder)](https://ci.appveyor.com/project/ryanoneill1970/genegenie-geocoder/history)

## Contributing

We would love your help, see [Contributing.md](Contributing.md) for guidelines.
