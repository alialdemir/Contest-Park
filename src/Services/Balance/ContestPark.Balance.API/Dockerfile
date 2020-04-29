FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Balance/ContestPark.Balance.API/ContestPark.Balance.API.csproj", "src/Services/Balance/ContestPark.Balance.API/"]
RUN dotnet restore "src/Services/Balance/ContestPark.Balance.API/ContestPark.Balance.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Balance/ContestPark.Balance.API"
RUN dotnet build "ContestPark.Balance.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Balance.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Balance.API.dll"]