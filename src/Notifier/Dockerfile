#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifier/FeedTracker.Notifier.csproj", "Notifier/"]
COPY ["Shared/FeedTracker.Shared.csproj", "Shared/"]
RUN dotnet restore "./Notifier/./FeedTracker.Notifier.csproj"
COPY . .
WORKDIR "/src/Notifier"
RUN dotnet build "./FeedTracker.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FeedTracker.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FeedTracker.Notifier.dll"]