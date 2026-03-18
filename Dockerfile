FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["ConexyTask.csproj", "./"]
RUN dotnet restore "ConexyTask.csproj"

# Копируем всё остальное и собираем
COPY . .
RUN dotnet publish "ConexyTask.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Порт для Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ConexyTask.dll"]