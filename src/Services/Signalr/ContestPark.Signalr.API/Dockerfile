FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Services/Signalr/ContestPark.Signalr.API/ContestPark.Signalr.API.csproj", "src/Services/Signalr/ContestPark.Signalr.API/"]
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