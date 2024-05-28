FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ecommerce.BackgroundWorkers/Ecommerce.BackgroundWorkers.csproj", "src/Ecommerce.BackgroundWorkers/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
RUN dotnet restore "src/Ecommerce.BackgroundWorkers/Ecommerce.BackgroundWorkers.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.BackgroundWorkers"
RUN dotnet build "Ecommerce.BackgroundWorkers.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecommerce.BackgroundWorkers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.BackgroundWorkers.dll"]
