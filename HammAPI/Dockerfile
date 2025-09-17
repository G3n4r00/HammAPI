# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiando csproj e restaurando dependencias
COPY ["HammAPI.csproj", "."]
RUN dotnet restore "HammAPI.csproj"

# Copiar tudo e fazer o build
COPY . .
RUN dotnet build "HammAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HammAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
COPY --from=publish /app/publish .

# Abrir o port 80 para http
EXPOSE 80

# Ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "HammAPI.dll"]
