#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Ecommerce.Admin.HttpApi.Host/Ecommerce.Admin.HttpApi.Host.csproj", "src/Ecommerce.Admin.HttpApi.Host/"]
COPY ["src/Ecommerce.Admin.Application/Ecommerce.Admin.Application.csproj", "src/Ecommerce.Admin.Application/"]
COPY ["src/Ecommerce.Admin.Application.Contracts/Ecommerce.Admin.Application.Contracts.csproj", "src/Ecommerce.Admin.Application.Contracts/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Admin.HttpApi/Ecommerce.Admin.HttpApi.csproj", "src/Ecommerce.Admin.HttpApi/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
RUN dotnet restore "src/Ecommerce.Admin.HttpApi.Host/Ecommerce.Admin.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.Admin.HttpApi.Host"
RUN dotnet build "Ecommerce.Admin.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.Admin.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Admin.HttpApi.Host.dll"]
