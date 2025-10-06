# Restore and build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

LABEL org.opencontainers.image.source=https://github.com/blaj/metin2-server-channel

ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

RUN --mount=type=secret,id=github_token \
    --mount=type=secret,id=github_username \
    export GITHUB_TOKEN=$(cat /run/secrets/github_token) && \
    export GITHUB_USERNAME=$(cat /run/secrets/github_username) && \
    dotnet nuget add source "https://nuget.pkg.github.com/${GITHUB_USERNAME}/index.json" \
        --name github \
        --username "$GITHUB_USERNAME" \
        --password "$GITHUB_TOKEN" \
        --store-password-in-clear-text

WORKDIR /src

COPY Metin2Server.Shared/Metin2Server.Shared.csproj Metin2Server.Shared/
COPY Metin2Server.Network/Metin2Server.Network.csproj Metin2Server.Network/
COPY Metin2Server.Domain/Metin2Server.Domain.csproj Metin2Server.Domain/
COPY Metin2Server.Channel/Metin2Server.Channel.csproj Metin2Server.Channel/

RUN dotnet restore "./Metin2Server.Channel/Metin2Server.Channel.csproj" --arch $TARGETARCH

COPY . ./

RUN dotnet build "./Metin2Server.Channel/Metin2Server.Channel.csproj" -c $BUILD_CONFIGURATION -o /app/build --arch $TARGETARCH

# Publish
FROM build AS publish

ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

RUN dotnet publish "./Metin2Server.Channel/Metin2Server.Channel.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --arch $TARGETARCH

# Entrypoint
FROM mcr.microsoft.com/dotnet/aspnet:8.0

USER app

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Metin2Server.Channel.dll"]
