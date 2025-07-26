FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["REPM.API/REPM.API.csproj", "REPM.API/"]
COPY ["REPM.Application/REPM.Application.csproj", "REPM.Application/"]
COPY ["REPM.Domain/REPM.Domain.csproj", "REPM.Domain/"]
COPY ["REPM.Infrastructure/REPM.Infrastructure.csproj", "REPM.Infrastructure/"]
RUN dotnet restore "REPM.API/REPM.API.csproj"
COPY . .
WORKDIR "/src/REPM.API"
RUN dotnet build "./REPM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./REPM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "REPM.API.dll"]
