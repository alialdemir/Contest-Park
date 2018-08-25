FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Services/Cp/ContestPark.Cp.API/ContestPark.Cp.API.csproj", "src/Services/Cp/ContestPark.Cp.API/"]
COPY ["src/Domain/Cp/ContestPark.Domain.Cp/ContestPark.Domain.Cp.csproj", "src/Domain/Cp/ContestPark.Domain.Cp/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core.Domain/ContestPark.Core.Domain.csproj", "src/BuildingBlocks/Core/ContestPark.Core.Domain/"]
COPY ["src/BuildingBlocks/Core/ContestPark.Core.OrleansClient/ContestPark.Core.OrleansClient.csproj", "src/BuildingBlocks/Core/ContestPark.Core.OrleansClient/"]
COPY ["src/BuildingBlocks/Core/New folder/ContestPark.Core.Service/ContestPark.Core.Service.csproj", "src/BuildingBlocks/Core/New folder/ContestPark.Core.Service/"]
RUN dotnet restore "src/Services/Cp/ContestPark.Cp.API/ContestPark.Cp.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Cp/ContestPark.Cp.API"
RUN dotnet build "ContestPark.Cp.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Cp.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Cp.API.dll"]