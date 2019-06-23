FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/Services/Chat/ContestPark.Chat.API/ContestPark.Chat.API.csproj", "src/Services/Chat/ContestPark.Chat.API/"]
RUN dotnet restore "src/Services/Chat/ContestPark.Chat.API/ContestPark.Chat.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Chat/ContestPark.Chat.API"
RUN dotnet build "ContestPark.Chat.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Chat.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Chat.API.dll"]