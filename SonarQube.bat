dotnet tool install --global dotnet-sonarscanner
dotnet test GeneGenie.Geocoder.Tests\GeneGenie.Geocoder.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="%CD%\opencover.xml"
dotnet build-server shutdown
dotnet sonarscanner begin /k:"GeneGenie.Geocoder" /o:"thegenegenieproject" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login=%SonarQubeApiKey% /d:sonar.cs.opencover.reportsPaths="%CD%\opencover.xml"
dotnet build
dotnet sonarscanner end /d:sonar.login=%SonarQubeApiKey%
