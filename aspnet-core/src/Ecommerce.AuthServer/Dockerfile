#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Ecommerce.AuthServer/Ecommerce.AuthServer.csproj", "src/Ecommerce.AuthServer/"]
COPY ["src/Ecommerce.EntityFrameworkCore/Ecommerce.EntityFrameworkCore.csproj", "src/Ecommerce.EntityFrameworkCore/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Domain.Shared/Ecommerce.Domain.Shared.csproj", "src/Ecommerce.Domain.Shared/"]
RUN dotnet restore "src/Ecommerce.AuthServer/Ecommerce.AuthServer.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.AuthServer"
RUN dotnet build "Ecommerce.AuthServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.AuthServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.AuthServer.dll"]