# ============================================
# Build Stage
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILDCONFIG=Release
WORKDIR /src

# Copy project file from apps/api folder
COPY ["apps/api/newsApi.csproj", "apps/api/"]

# Restore dependencies
RUN dotnet restore "apps/api/newsApi.csproj"

# Copy all backend source code
COPY apps/api/ apps/api/

# Build the application
WORKDIR "/src/apps/api"
RUN dotnet build "newsApi.csproj" \
    -c $BUILDCONFIG \
    -o /app/build \
    --no-restore

# ============================================
# Publish Stage
# ============================================
FROM build AS publish
ARG BUILDCONFIG=Release
WORKDIR "/src/apps/api"
RUN dotnet publish "newsApi.csproj" \
    -c $BUILDCONFIG \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================
# Runtime Stage
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Install curl for healthchecks
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r newsapi && \
    useradd -r -g newsapi newsapi

WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R newsapi:newsapi /app

# Switch to non-root user
USER newsapi

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "newsApi.dll"]
