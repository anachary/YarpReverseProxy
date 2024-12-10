# Stage 1: Base image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./ 
RUN dotnet restore

# Copy all project files and build
COPY . ./ 
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Copy published artifacts from build stage
COPY --from=build /app/publish . 

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production 
ENV ASPNETCORE_URLS=http://+:5274

# Expose port for the application
EXPOSE 5274

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser

# Health check to verify application is running
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s \
  CMD curl -f http://localhost:5274/health || exit 1

# Application startup command
ENTRYPOINT ["dotnet", "YarpReverseProxy.dll"]
