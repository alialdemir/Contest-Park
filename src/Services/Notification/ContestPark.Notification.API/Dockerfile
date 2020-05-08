#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Notification/ContestPark.Notification.API/ContestPark.Notification.API.csproj", "src/Services/Notification/ContestPark.Notification.API/"]
RUN dotnet restore "src/Services/Notification/ContestPark.Notification.API/ContestPark.Notification.API.csproj"
COPY . .
WORKDIR "/src/Services/Notification/ContestPark.Notification.API"
RUN dotnet build "ContestPark.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContestPark.Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContestPark.Notification.API.dll"]