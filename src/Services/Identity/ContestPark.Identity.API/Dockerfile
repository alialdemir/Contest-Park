FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Identity/ContestPark.Identity.API/ContestPark.Identity.API.csproj", "src/Services/Identity/ContestPark.Identity.API/"]
COPY ["src/BuildingBlocks/EventBus/ContestPark.EventBus.IntegrationEventLogEF/ContestPark.EventBus.IntegrationEventLogEF.csproj", "src/BuildingBlocks/EventBus/ContestPark.EventBus.IntegrationEventLogEF/"]
COPY ["src/BuildingBlocks/EventBus/ContestPark.EventBus/ContestPark.EventBus.csproj", "src/BuildingBlocks/EventBus/ContestPark.EventBus/"]
COPY ["src/BuildingBlocks/EventBus/ContestPark.EventBus.RabbitMQ/ContestPark.EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/ContestPark.EventBus.RabbitMQ/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core/ContestPark.Core.csproj", "src/BuildingBlocks/Core/ContestPark.Core/"]
RUN dotnet restore "src/Services/Identity/ContestPark.Identity.API/ContestPark.Identity.API.csproj"
COPY . .
WORKDIR "/src/Services/Identity/ContestPark.Identity.API"
RUN dotnet build "ContestPark.Identity.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Identity.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Identity.API.dll"]