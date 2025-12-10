# Docker Compose Setup for Todo App

This project uses Docker Compose to orchestrate three containerized services:

## Services

1. **MSSQL Database** (`mssql`)
   - SQL Server 2022 Developer Edition
   - Port: 1433
   - Volume: `mssql_data` for persistent storage

2. **.NET Backend API** (`api`)
   - ASP.NET Core 8.0 REST API
   - Port: 5000 (mapped to 8080 internal)
   - Auto-runs EF Core migrations on startup
   - Communicates with MSSQL and React frontend

3. **React Frontend** (`frontend`)
   - React with Vite build
   - Served by Nginx
   - Port: 80
   - Proxy: `/api/*` requests routed to backend

## Prerequisites

- Docker Desktop (Windows, Mac) or Docker Engine (Linux)
- Docker Compose (included with Docker Desktop)
- 4GB+ available RAM

## Quick Start

### 1. Build and Start Services

```bash
docker-compose up --build
```

This will:
- Build the .NET API image
- Build the React frontend image
- Pull MSSQL 2022 image
- Start all three services
- Run database migrations automatically

### 2. Access the Application

- **Frontend**: http://localhost
- **API**: http://localhost:5000
- **Database**: localhost:1433 (SQL Server Management Studio or Azure Data Studio)
  - Username: `sa`
  - Password: `YourPassword123!`

## Common Commands

```bash
# Start in background
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
docker-compose logs -f mssql
docker-compose logs -f frontend

# Rebuild services
docker-compose up -d --build

# Remove volumes (deletes database)
docker-compose down -v

# Stop and remove everything
docker-compose down --volumes --remove-orphans
```

## Environment Variables

Edit `docker-compose.yml` to change:
- `SA_PASSWORD`: MSSQL admin password
- `ASPNETCORE_ENVIRONMENT`: Production or Development
- Port mappings in the `ports` section

## Troubleshooting

### MSSQL won't connect
```bash
# Wait a few seconds for MSSQL to fully initialize
# Check logs
docker-compose logs mssql

# May need to wait 30+ seconds on first start
```

### API can't connect to database
```bash
# Check connection string in logs
docker-compose logs api

# Verify MSSQL is healthy
docker-compose ps

# Database migrations fail - check logs
docker-compose logs api | grep -i "migration\|error"
```

### Frontend shows blank/errors
```bash
# Check frontend logs
docker-compose logs frontend

# Verify API is accessible
curl http://localhost:5000/api/tasks

# Check nginx proxy configuration in frontend/nginx.conf
```

### Port already in use
Change port mappings in `docker-compose.yml`:
```yaml
ports:
  - "8000:8080"  # API on 8000 instead of 5000
  - "3000:80"    # Frontend on 3000 instead of 80
```

## Network Communication

Services communicate via the `todoapp_network` bridge network:
- Frontend Nginx can reach API at: `http://api:8080`
- API can reach MSSQL at: `mssql:1433`

## Development Workflow

### Making API Changes

1. Edit backend code
2. Rebuild the API image:
   ```bash
   docker-compose up -d --build api
   ```

### Making Frontend Changes

1. Edit frontend code
2. Rebuild the frontend image:
   ```bash
   docker-compose up -d --build frontend
   ```

### Database Changes (EF Core)

1. Create migration locally (or just update `DbContext`)
2. The entrypoint script runs `dotnet ef database update` automatically
3. Rebuild API: `docker-compose up -d --build api`

## Production Considerations

For production deployments, consider:

- Change database password (`SA_PASSWORD`)
- Set `ASPNETCORE_ENVIRONMENT: Production`
- Use Docker secrets for sensitive data
- Remove port mappings or restrict access
- Add health checks to frontend service
- Use proper SSL/TLS certificates
- Scale services with Docker Swarm or Kubernetes
- Use managed database services instead of containerized MSSQL

## Security Notes

- Default password is weak - change in `docker-compose.yml`
- Database exposed on localhost:1433
- No authentication between frontend and API
- Use secrets management in production
