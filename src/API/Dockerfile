# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8082

# Install debugger
RUN apt-get update && apt-get install -y curl binutils \
    && mkdir -p /root/.vs-debugger \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /root/.vs-debugger

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
ARG SOURCE_ROOT=/src

COPY .editorconfig .
WORKDIR /src

# Copy only .csproj files first to leverage Docker layer caching
COPY ["Directory.Build.props", "."]
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "Common/Common.Infrastructure/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "Common/Common.Application/"]
COPY ["src/Common/Common.Domain/Common.Domain.csproj", "Common/Common.Domain/"]
COPY ["src/Modules/Users/Modules.Users.Infrastructure/Modules.Users.Infrastructure.csproj", "Modules/Users/Modules.Users.Infrastructure/"]
COPY ["src/Modules/Users/Modules.Users.Application/Modules.Users.Application.csproj", "Modules/Users/Modules.Users.Application/"]
COPY ["src/Modules/Users/Modules.Users.Domain/Modules.Users.Domain.csproj", "Modules/Users/Modules.Users.Domain/"]

COPY ["src/Modules/Accounts/Modules.Accounts.Infrastructure/Modules.Accounts.Infrastructure.csproj", "Modules/Accounts/Modules.Accounts.Infrastructure/"]
COPY ["src/Modules/Accounts/Modules.Accounts.Application/Modules.Accounts.Application.csproj", "Modules/Accounts/Modules.Accounts.Application/"]
COPY ["src/Modules/Accounts/Modules.Accounts.Domain/Modules.Accounts.Domain.csproj", "Modules/Accounts/Modules.Accounts.Domain/"]
COPY src/API/appsettings.Development.json .
# Restore dependencies
RUN dotnet restore "./API/API.csproj"

# Copy entire source
COPY src .

# Set working dir directly to /src/API 
WORKDIR /src/API


# ⚠️ KEY: Build with full source path mapping
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build \
  /p:PathMap=/src=${SOURCE_ROOT} \
  /p:EmbedAllSources=true \
  /p:SourceLinkCreate=true

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
  /p:PathMap=/src=${SOURCE_ROOT} \
  /p:EmbedAllSources=true \
  /p:SourceLinkCreate=true \
  /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
