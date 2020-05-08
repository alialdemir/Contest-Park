FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Signalr/ContestPark.Signalr.API/ContestPark.Signalr.API.csproj", "src/Services/Signalr/ContestPark.Signalr.API/"]
COPY ["src/BuildingBlocks/EventBus/ContestPark.EventBus/ContestPark.EventBus.csproj", "src/BuildingBlocks/EventBus/ContestPark.EventBus/"]
COPY ["src/BuildingBlocks/EventBus/ContestPark.EventBus.RabbitMQ/ContestPark.EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/ContestPark.EventBus.RabbitMQ/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core/ContestPark.Core.csproj", "src/BuildingBlocks/Core/ContestPark.Core/"]
RUN dotnet restore "src/Services/Signalr/ContestPark.Signalr.API/ContestPark.Signalr.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Signalr/ContestPark.Signalr.API"
RUN dotnet build "ContestPark.Signalr.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Signalr.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Signalr.API.dll"]