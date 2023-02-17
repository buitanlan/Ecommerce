#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Ecommerce.HttpApi.Host/Ecommerce.HttpApi.Host.csproj", "src/Ecommerce.HttpApi.Host/"]
COPY ["src/Ecommerce.Application/Ecommerce.Application.csproj", "src/Ecommerce.Application/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
COPY ["src/Ecommerce.Application.Contracts/Ecommerce.Application.Contracts.csproj", "src/Ecommerce.Application.Contracts/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
COPY ["src/Ecommerce.HttpApi/Ecommerce.HttpApi.csproj", "src/Ecommerce.HttpApi/"]
RUN dotnet restore "src/Ecommerce.HttpApi.Host/Ecommerce.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.HttpApi.Host"
RUN dotnet build "Ecommerce.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.HttpApi.Host.dll"]