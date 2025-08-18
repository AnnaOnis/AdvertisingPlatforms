# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AdvertisingPlatforms.WebAPI/AdvertisingPlatforms.Web.csproj", "AdvertisingPlatforms.WebAPI/"]
COPY ["AdvertisingPlatform.Domain/AdvertisingPlatforms.Domain.csproj", "AdvertisingPlatform.Domain/"]
COPY ["AdvertisingPlatforms.DAL/AdvertisingPlatforms.DAL.csproj", "AdvertisingPlatforms.DAL/"]
COPY ["AdvertisingPlatforms.Base/AdvertisingPlatforms.Base.csproj", "AdvertisingPlatforms.Base/"]
RUN dotnet restore "./AdvertisingPlatforms.WebAPI/AdvertisingPlatforms.Web.csproj"
RUN dotnet restore "./Tools/DataGenerator/DataGenerator/DataGenerator.csproj"
COPY . .
WORKDIR "/src/AdvertisingPlatforms.WebAPI"
RUN dotnet build "./AdvertisingPlatforms.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR "/src/Tools/DataGenerator/DataGenerator"
RUN dotnet build "./DataGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/build/DataGenerator

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdvertisingPlatforms.WebAPI/AdvertisingPlatforms.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "./Tools/DataGenerator/DataGenerator/DataGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/publish/DataGenerator /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/DataGenerator ./DataGenerator
COPY ./run_all.sh .
RUN chmod +x ./run_all.sh
ENTRYPOINT ["./run_all.sh"]