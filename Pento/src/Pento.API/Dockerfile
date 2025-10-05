# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Pento.API/Pento.API.csproj", "src/Pento.API/"]
COPY ["src/Pento.Infrastructure/Pento.Infrastructure.csproj", "src/Pento.Infrastructure/"]
COPY ["src/Pento.Application/Pento.Application.csproj", "src/Pento.Application/"]
COPY ["src/Pento.Domain/Pento.Domain.csproj", "src/Pento.Domain/"]
RUN dotnet restore "./src/Pento.API/Pento.API.csproj"
COPY . .
WORKDIR "/src/src/Pento.API"
RUN dotnet build "./Pento.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pento.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pento.API.dll"]