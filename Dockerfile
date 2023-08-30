# =============================================================================
# Author: Vladyslav Zaiets | https://sarmkadan.com
# CTO & Software Architect
# =============================================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

# Copy project file
COPY ["BlazorComponentLibrary.csproj", ""]

# Restore dependencies
RUN dotnet restore "BlazorComponentLibrary.csproj"

# Copy source code
COPY . .

# Build library
RUN dotnet build "BlazorComponentLibrary.csproj" -c Release -o /app/build

# Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Copy built artifacts
COPY --from=builder /app/build .

# Expose ports
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "BlazorComponentLibrary.dll"]
