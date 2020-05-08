FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Category/ContestPark.Category.API/ContestPark.Category.API.csproj", "src/Services/Category/ContestPark.Category.API/"]
RUN dotnet restore "src/Services/Category/ContestPark.Category.API/ContestPark.Category.API.csproj"
COPY . .
WORKDIR "/src/Services/Category/ContestPark.Category.API"
RUN dotnet build "ContestPark.Category.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Category.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Category.API.dll"]