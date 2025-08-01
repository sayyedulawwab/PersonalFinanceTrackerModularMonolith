# Use official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install dependencies
RUN apt-get update \
    && apt-get install -y curl unzip \
    && rm -rf /var/lib/apt/lists/*

# Install vsdbg for VS Code debugging
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Set working directory (matches "workspaceFolder" in devcontainer.json)
WORKDIR /workspace

# Expose ports
EXPOSE 8080 8081 8082 5341

# Prevent container from exiting (used only for VS Code attachment/debug)
CMD [ "sleep", "infinity" ]