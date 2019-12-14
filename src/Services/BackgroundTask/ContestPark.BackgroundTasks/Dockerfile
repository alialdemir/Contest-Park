FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/Services/BackgroundTask/ContestPark.BackgroundTasks/ContestPark.BackgroundTasks.csproj", "src/Services/BackgroundTask/ContestPark.BackgroundTasks/"]
RUN dotnet restore "src/Services/BackgroundTask/ContestPark.BackgroundTasks/ContestPark.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/src/Services/BackgroundTask/ContestPark.BackgroundTasks"
RUN dotnet build "ContestPark.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContestPark.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContestPark.BackgroundTasks.dll"]