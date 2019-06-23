FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/ApiGateways/ContestPark.OcelotApiGw/ContestPark.OcelotApiGw.csproj", "src/ApiGateways/ContestPark.OcelotApiGw/"]
RUN dotnet restore "src/ApiGateways/ContestPark.OcelotApiGw/ContestPark.OcelotApiGw.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/ContestPark.OcelotApiGw"
RUN dotnet build "ContestPark.OcelotApiGw.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.OcelotApiGw.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.OcelotApiGw.dll"]