FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["./src/CityNexus.Modulith.Api/CityNexus.Modulith.Api.csproj", "src/CityNexus.Modulith.Api/"]
RUN dotnet restore "src/CityNexus.Modulith.Api/CityNexus.Modulith.Api.csproj"
COPY ./src  .
WORKDIR "/src/CityNexus.Modulith.Api"
RUN dotnet build "CityNexus.Modulith.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CityNexus.Modulith.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityNexus.Modulith.Api.dll"]
