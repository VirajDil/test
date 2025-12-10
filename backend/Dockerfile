# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj and restore dependencies
COPY ["todoapp_backend.csproj", "./"]
RUN dotnet restore "todoapp_backend.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "todoapp_backend.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "todoapp_backend.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Install SQL Server tools for migrations
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 8080
EXPOSE 8081

CMD ["dotnet", "todoapp_backend.dll"]
