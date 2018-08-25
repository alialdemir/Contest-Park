FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/BuildingBlocks/ApiGateway/ContestPark.OcelotApiGw/ContestPark.OcelotApiGw.csproj", "src/BuildingBlocks/ApiGateway/ContestPark.OcelotApiGw/"]
RUN dotnet restore "src/BuildingBlocks/ApiGateway/ContestPark.OcelotApiGw/ContestPark.OcelotApiGw.csproj"
COPY . .
WORKDIR "/src/src/BuildingBlocks/ApiGateway/ContestPark.OcelotApiGw"
RUN dotnet build "ContestPark.OcelotApiGw.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.OcelotApiGw.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.OcelotApiGw.dll"]