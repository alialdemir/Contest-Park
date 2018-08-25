FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/Identity/ContestPark.Identity.API/ContestPark.Identity.API.csproj src/Services/Identity/ContestPark.Identity.API/
RUN dotnet restore src/Services/Identity/ContestPark.Identity.API/ContestPark.Identity.API.csproj
COPY . .
WORKDIR /src/src/Services/Identity/ContestPark.Identity.API
RUN dotnet build ContestPark.Identity.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ContestPark.Identity.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Identity.API.dll"]
