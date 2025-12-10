# Todo App - Docker Setup Guide

A full-stack Todo application with a .NET 8 backend API and React/TypeScript frontend, running in Docker containers.

## Project Structure

```
├── backend/              # .NET 8 ASP.NET Core API
├── frontend/             # React + TypeScript + Vite
├── docker-compose.yml    # Docker Compose configuration
└── README.md             # This file
```

## Prerequisites

Before running the application, ensure you have installed:

- **Docker** (v20.10+) - [Download](https://www.docker.com/products/docker-desktop)
- **Docker Compose** (v2.0+) - Usually included with Docker Desktop
- **Git** - [Download](https://git-scm.com/)

### Verify Installation

```powershell
docker --version
docker-compose --version
```

## Quick Start

### 1. Clone the Repository

```powershell
git clone <your-repo-url>
cd test
```

### 2. Start All Services

```powershell
docker-compose up -d
```

This will:
- Build the backend API image
- Build the frontend image
- Start the MSSQL database
- Start the API server
- Start the frontend server

### 3. Access the Application

- **Frontend:** http://localhost
- **Backend API:** http://localhost:5000
- **Swagger Documentation:** http://localhost:5000/swagger

---

## Docker Commands Reference

### Start Services

**Start in background (detached mode):**
```powershell
docker-compose up -d
```

**Start with logs visible (foreground):**
```powershell
docker-compose up
```

**Rebuild and start:**
```powershell
docker-compose up -d --build
```

### Stop Services

**Stop all services (keep data):**
```powershell
docker-compose down
```

**Stop and remove volumes (delete database):**
```powershell
docker-compose down -v
```

### View Logs

**View all service logs:**
```powershell
docker-compose logs -f
```

**View specific service logs:**
```powershell
docker-compose logs -f api      # Backend API
docker-compose logs -f frontend # Frontend
docker-compose logs -f mssql    # Database
```

**View last 50 lines:**
```powershell
docker-compose logs --tail=50 api
```

### Check Status

**View running containers:**
```powershell
docker-compose ps
```

### Execute Commands

**Run command in backend container:**
```powershell
docker-compose exec backend dotnet ef database update
docker-compose exec backend dotnet run
```

**Access SQL Server in container:**
```powershell
docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlPass@123456
```

---

## Services Configuration

### MSSQL Database
- **Container:** `todoapp_mssql`
- **Port:** `1433`
- **Username:** `sa`
- **Password:** `SqlPass@123456`
- **Database:** `TodoDb`
- **Health Check:** Enabled (40s startup period)

### Backend API
- **Container:** `todoapp_api_new`
- **Port:** `5000` (maps to container port `8080`)
- **Framework:** .NET 8 ASP.NET Core
- **Features:** 
  - Entity Framework Core with SQL Server
  - Swagger/OpenAPI documentation
  - CORS enabled for all origins
  - Auto-creates database on startup

### Frontend
- **Container:** `todoapp_frontend`
- **Port:** `80`
- **Framework:** React + TypeScript + Vite
- **Server:** Nginx

---

## Troubleshooting

### Container Name Already in Use

If you get: `Error: The container name "/todoapp_mssql" is already in use`

**Solution:**
```powershell
docker rm -f todoapp_mssql todoapp_api_new todoapp_frontend
docker-compose up -d
```

Or completely clean up:
```powershell
docker-compose down -v
docker-compose up -d
```

### Port Already in Use

If ports 80, 5000, or 1433 are already in use, edit `docker-compose.yml`:

```yaml
ports:
  - "8080:8080"  # Change 5000 to 8080
```

### Database Connection Failed

Ensure the SQL Server container is running and healthy:

```powershell
docker-compose ps
```

Wait 40+ seconds for the health check to pass before API connects.

### API Container Exits Immediately

Check logs:
```powershell
docker-compose logs api
```

Common issues:
- Connection string mismatch (should be `SqlPass@123456`)
- Database not ready - wait for health check to pass
- Missing environment variables in `docker-compose.yml`

### Frontend Shows Blank Page

Check browser console and frontend logs:
```powershell
docker-compose logs -f frontend
```

Ensure API is accessible: `http://localhost:5000/swagger`

---

## Development Workflow

### Without Docker (Local Development)

**Backend:**
```powershell
cd backend
dotnet restore
dotnet ef database update
dotnet run
# API available at http://localhost:5000
```

**Frontend:**
```powershell
cd frontend
npm install
npm run dev
# Frontend available at http://localhost:5173
```

### With Docker (Production-like)

```powershell
# Start all services
docker-compose up -d

# View logs while developing
docker-compose logs -f api
```

---

## Database Management

### View Database Tables

```powershell
docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlPass@123456 -d TodoDb
```

### Backup Database

```powershell
docker-compose exec mssql /bin/bash -c "cd /var/opt/mssql/backup && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlPass@123456 -Q 'BACKUP DATABASE TodoDb TO DISK = '\''/var/opt/mssql/backup/TodoDb.bak'\'';'"
```

### Reset Database

```powershell
docker-compose down -v
docker-compose up -d
```

---

## Environment Variables

Edit `docker-compose.yml` to change:

- `SA_PASSWORD` - SQL Server admin password
- `ASPNETCORE_ENVIRONMENT` - Backend environment (Production/Development)
- `ASPNETCORE_URLS` - Backend listening URL
- Connection strings in `ConnectionStrings__DefaultConnection`

---

## File Structure

### Backend (`backend/`)
```
backend/
├── Controllers/          # API endpoints
├── Data/                 # Database context
├── DTOs/                 # Data transfer objects
├── Models/               # Domain models
├── Repositories/         # Data access layer
├── Services/             # Business logic
├── Program.cs            # Application startup
├── appsettings.json      # Configuration
├── Dockerfile            # Docker image definition
└── todoapp_backend.csproj# Project file
```

### Frontend (`frontend/`)
```
frontend/
├── src/
│   ├── components/       # React components
│   ├── services/         # API client
│   ├── types/            # TypeScript types
│   ├── main.tsx          # Entry point
│   └── style.css         # Styles
├── package.json          # NPM dependencies
├── vite.config.ts        # Vite configuration
├── Dockerfile            # Docker image definition
└── nginx.conf            # Nginx configuration
```

---

## Performance Tips

1. **Rebuild Images Only When Needed:**
   ```powershell
   docker-compose up -d  # Uses cached images
   docker-compose up -d --build  # Rebuilds images
   ```

2. **Free Up Disk Space:**
   ```powershell
   docker system prune -a
   docker volume prune
   ```

3. **Monitor Container Resources:**
   ```powershell
   docker stats
   ```

4. **Use Detached Mode:**
   ```powershell
   docker-compose up -d  # Better for production
   docker-compose up     # Better for debugging
   ```

---

## API Documentation

Once the application is running, access the interactive API documentation:

**Swagger UI:** http://localhost:5000/swagger

Available endpoints:
- `GET /api/tasks` - Get all tasks
- `POST /api/tasks` - Create a new task
- `GET /api/tasks/{id}` - Get task by ID
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

---

## Common Issues Checklist

- [ ] Docker Desktop is running
- [ ] Ports 80, 5000, 1433 are not in use
- [ ] `.gitignore` file exists and prevents `node_modules/`, `bin/`, `obj/`
- [ ] `docker-compose.yml` connection string matches `appsettings.json`
- [ ] SQL Server health check passed (wait 40+ seconds)
- [ ] API container started without errors

---

## Support & Help

For issues, check:
1. Container logs: `docker-compose logs`
2. Docker status: `docker-compose ps`
3. Network: `docker network ls`
4. Volumes: `docker volume ls`

---

## License

[Add your license here]

---

## Contributors

- [Your Name/Team]

---

**Last Updated:** December 10, 2025
