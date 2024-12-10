
# YARP Reverse Proxy

This project implements a reverse proxy using **YARP (Yet Another Reverse Proxy)** in .NET 9. It is designed to handle and forward HTTP requests to various backend services while also managing load balancing, routing, and health checks.

## Table of Contents

- [Project Overview](#project-overview)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Build and Run Locally](#build-and-run-locally)
  - [Docker](#docker)
  - [Deploy to AKS](#deploy-to-aks)
- [Configuration](#configuration)
- [Health Checks](#health-checks)
- [License](#license)

## Project Overview

The YARP reverse proxy serves as an intermediary between clients and backend services. It supports features like:

- Load balancing
- Dynamic routing
- Health checks
- Dockerized container deployment
- Kubernetes deployment on Azure Kubernetes Service (AKS)

This project uses **.NET 9**, **YARP**, and **Docker** to build the reverse proxy and deploy it efficiently.

## Prerequisites

- **.NET 9 SDK** – for building and running the application locally
- **Docker** – for containerizing the application
- **Azure CLI** – for deploying the container to Azure Kubernetes Service (AKS)
- **kubectl** – Kubernetes CLI tool for managing resources in AKS
- **Azure Kubernetes Service (AKS)** – a cloud-based Kubernetes service from Azure

## Getting Started

### Build and Run Locally

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd <repository-directory>
   ```

2. Restore project dependencies:

   ```bash
   dotnet restore
   ```

3. Build and run the application locally:

   ```bash
   dotnet run
   ```

   By default, the application will run on port `5274`. You can access the reverse proxy at `http://localhost:5274`.

### Docker

To containerize the application, follow these steps:

1. Build the Docker image:

   ```bash
   docker build -t yarp-reverse-proxy .
   ```

2. Run the Docker container:

   ```bash
   docker run -p 5274:5274 yarp-reverse-proxy
   ```

   The reverse proxy will be accessible at `http://localhost:5274` inside the container.

### Deploy to AKS

Before deploying to AKS, make sure to push the Docker image to a container registry, such as **Azure Container Registry (ACR)** or **Docker Hub**.

1. **Login to Azure** and configure `kubectl` to use your AKS cluster:

   ```bash
   az login
   az aks get-credentials --resource-group <your-resource-group> --name <your-aks-cluster-name>
   ```

2. **Build and Push Docker Image to a Registry**:

   ```bash
   # Build the Docker image
   docker build -t <your-registry>/yarp-reverse-proxy:latest .

   # Push the image to your container registry
   docker push <your-registry>/yarp-reverse-proxy:latest
   ```

3. **Deploy to AKS**:

   Apply the Kubernetes configuration files to deploy your application.

   ```bash
   kubectl apply -f deployment.yaml
   kubectl apply -f service.yaml
   ```

4. **Check the Status of the Deployment**:

   Monitor the status of your pods and services:

   ```bash
   kubectl get pods
   kubectl get svc
   ```

5. **Access the Service**:

   If you're using a `LoadBalancer` service type, obtain the external IP address:

   ```bash
   kubectl get svc yarp-reverse-proxy-service
   ```

   Access the reverse proxy at `http://<external-ip>:80`.

## Configuration

The reverse proxy is configured to listen on port `5274`. You can adjust the port bindings in the Dockerfile and the Kubernetes configuration.

Key environment variables in the application:

- **ASPNETCORE_URLS**: The URL the reverse proxy listens to (e.g., `http://+:5274`).

You can modify these in the **Dockerfile** or **Kubernetes YAML files** as needed.

## Health Checks

The application includes **health checks** to ensure that it is running and ready to handle requests.

- **Liveness Probe**: Verifies if the reverse proxy is alive.
- **Readiness Probe**: Ensures the reverse proxy is ready to receive traffic.

These checks are performed at the `/health` endpoint on port `5274`.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
