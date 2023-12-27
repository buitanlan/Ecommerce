#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Ecommerce.Public.HttpApi.Host/Ecommerce.Public.HttpApi.Host.csproj", "src/Ecommerce.Public.HttpApi.Host/"]
COPY ["src/Ecommerce.Public.Application/Ecommerce.Public.Application.csproj", "src/Ecommerce.Public.Application/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
COPY ["src/Ecommerce.Public.Application.Contracts/Ecommerce.Public.Application.Contracts.csproj", "src/Ecommerce.Public.Application.Contracts/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
COPY ["src/Ecommerce.Public.HttpApi/Ecommerce.Public.HttpApi.csproj", "src/Ecommerce.Public.HttpApi/"]
RUN dotnet restore "src/Ecommerce.Public.HttpApi.Host/Ecommerce.Public.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.Public.HttpApi.Host"
RUN dotnet build "Ecommerce.Public.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.Public.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Public.HttpApi.Host.dll"]
