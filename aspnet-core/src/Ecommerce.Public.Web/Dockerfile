FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ecommerce.Public.Web/Ecommerce.Public.Web.csproj", "src/Ecommerce.Public.Web/"]
COPY ["src/Ecommerce.Public.Application/Ecommerce.Public.Application.csproj", "src/Ecommerce.Public.Application/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
COPY ["src/Ecommerce.Public.Application.Contracts/Ecommerce.Public.Application.Contracts.csproj", "src/Ecommerce.Public.Application.Contracts/"]
COPY ["src/Ecommerce.Public.HttpApi/Ecommerce.Public.HttpApi.csproj", "src/Ecommerce.Public.HttpApi/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
RUN dotnet restore "src/Ecommerce.Public.Web/Ecommerce.Public.Web.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.Public.Web"
RUN dotnet build "Ecommerce.Public.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecommerce.Public.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Public.Web.dll"]
