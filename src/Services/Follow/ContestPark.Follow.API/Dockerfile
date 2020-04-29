FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Follow/ContestPark.Follow.API/ContestPark.Follow.API.csproj", "src/Services/Follow/ContestPark.Follow.API/"]
RUN dotnet restore "src/Services/Follow/ContestPark.Follow.API/ContestPark.Follow.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Follow/ContestPark.Follow.API"
RUN dotnet build "ContestPark.Follow.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Follow.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Follow.API.dll"]