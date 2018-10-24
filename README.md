# GeneGenie.Geocoder
A .Net standard geocoder library that can query multiple backend geocoders and rotate between them (briefly named Neocoder, this is the new name for the project).

Currently supports Google and Bing geocoder APIs.

## Status
[![AppVeyor branch](https://img.shields.io/appveyor/ci/RyanONeill1970/genegenie-geocoder/master.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-geocoder) [![Codecov branch](https://img.shields.io/codecov/c/github/TheGeneGenieProject/GeneGenie.Geocoder/master.svg)](https://codecov.io/gh/TheGeneGenieProject/GeneGenie.Geocoder) [![AppVeyor tests](https://img.shields.io/appveyor/tests/RyanONeill1970/genegenie-geocoder.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-geocoder/build/tests)

[![Build stats](https://buildstats.info/appveyor/chart/ryanoneill1970/genegenie-geocoder)](https://ci.appveyor.com/project/ryanoneill1970/genegenie-geocoder/history)

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

