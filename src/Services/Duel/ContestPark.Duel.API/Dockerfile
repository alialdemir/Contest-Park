FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Services/Duel/ContestPark.Duel.API/ContestPark.Duel.API.csproj", "src/Services/Duel/ContestPark.Duel.API/"]
COPY ["src/Domain/Identity/ContestPark.Domain.Identity/ContestPark.Domain.Identity.csproj", "src/Domain/Identity/ContestPark.Domain.Identity/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core.Domain/ContestPark.Core.Domain.csproj", "src/BuildingBlocks/Core/ContestPark.Core.Domain/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core.OrleansClient/ContestPark.Core.OrleansClient.csproj", "src/BuildingBlocks/Core/ContestPark.Core.OrleansClient/"]
COPY ["src/BuildingBlocks/Core/New folder/ContestPark.Core.Service/ContestPark.Core.Service.csproj", "src/BuildingBlocks/Core/New folder/ContestPark.Core.Service/"]
COPY ["src/Domain/Duel/ContestPark.Domain.Duel/ContestPark.Domain.Duel.csproj", "src/Domain/Duel/ContestPark.Domain.Duel/"]
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