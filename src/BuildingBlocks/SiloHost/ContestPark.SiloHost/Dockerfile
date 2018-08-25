FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/BuildingBlocks/SiloHost/ContestPark.SiloHost/ContestPark.SiloHost.csproj src/BuildingBlocks/SiloHost/ContestPark.SiloHost/
RUN dotnet restore src/BuildingBlocks/SiloHost/ContestPark.SiloHost/ContestPark.SiloHost.csproj
COPY . .
WORKDIR /src/src/BuildingBlocks/SiloHost/ContestPark.SiloHost
RUN dotnet build ContestPark.SiloHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ContestPark.SiloHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.SiloHost.dll"]
