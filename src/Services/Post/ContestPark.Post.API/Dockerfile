FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Post/ContestPark.Post.API/ContestPark.Post.API.csproj", "src/Services/Post/ContestPark.Post.API/"]
RUN dotnet restore "src/Services/Post/ContestPark.Post.API/ContestPark.Post.API.csproj"
COPY . .
WORKDIR "/src/Services/Post/ContestPark.Post.API"
RUN dotnet build "ContestPark.Post.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContestPark.Post.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContestPark.Post.API.dll"]