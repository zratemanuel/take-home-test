#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 81 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fundo.Applications.WebApiSecurity/Fundo.Applications.WebApiSecurity.csproj", "Fundo.Applications.WebApiSecurity/"]
COPY ["Fundo.Applications.Domain/Fundo.Applications.Domain.csproj", "Fundo.Applications.Domain/"]
COPY ["Fundo.Applications.Repository/Fundo.Applications.Repository.csproj", "Fundo.Applications.Repository/"]
RUN dotnet restore "./Fundo.Applications.WebApiSecurity/Fundo.Applications.WebApiSecurity.csproj"
COPY . .
WORKDIR "/src/Fundo.Applications.WebApiSecurity"
RUN dotnet build "./Fundo.Applications.WebApiSecurity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fundo.Applications.WebApiSecurity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fundo.Applications.WebApiSecurity.dll"]