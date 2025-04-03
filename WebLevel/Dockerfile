# Используем образ ASP.NET для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8088


# Используем SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Устанавливаем git и клонируем репозиторий
RUN apt-get update && apt-get install -y git \
    && git clone https://<TOKEN>@github.com/trokhin87/Mail_Service.git myrepo

# Переходим в каталог с проектом
WORKDIR /src/myrepo/WebLevel

# Восстанавливаем зависимости и собираем проект
RUN dotnet restore WebLevel.csproj
RUN dotnet publish WebLevel.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ с минимальным размером
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV BaseUrl=uenv
ENV SmtpServer=uenv
ENV Port=uenv
ENV Password=uenv
ENV FromEmail=uenv

# Запускаем приложение
ENTRYPOINT ["dotnet", "WebLevel.dll"]
