FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Duel/ContestPark.Duel.API/ContestPark.Duel.API.csproj", "src/Services/Duel/ContestPark.Duel.API/"]
RUN dotnet restore "src/Services/Duel/ContestPark.Duel.API/ContestPark.Duel.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Duel/ContestPark.Duel.API"
RUN dotnet build "ContestPark.Duel.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Duel.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Duel.API.dll"]