# Todo

- Seed the DB using a startup seeder
- Generate migrations for the database using EF Core (Code-First approach using migrations)

---

# Requirements

- [MySQL](https://dev.mysql.com/downloads/mysql/) Community Server 8.0 or higher
- [Docker](https://www.docker.com/products/docker-desktop/) Desktop 4.79 or higher
- [Docker Compose](https://docs.docker.com/compose/install/) 5.1.4 or higher

---

## Initial Database Seed (temporary manual setup)

Run the following SQL to initialise and seed the database (this will later be replaced by EF Core migrations + startup seeder):
- [db_products_seed.sql](Resources/docker/mysql-init/db_products_seed.sql)

# Application Configuration
## User Secrets (Development only)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=3306;Database=productService;User=root;Password=yourMySqlPasswordHere;"
  }
}
```

## Production / Docker Environment Variables

When running in Docker or release mode, user secrets are not available. Use environment variables instead:

- `COMMERCEFABRIC_PRODUCTSERVICE_DB_HOST`
- `COMMERCEFABRIC_PRODUCTSERVICE_DB_PASSWORD`

Default values for other settings are defined in the appsettings.json, so are not required to be set.

## Running through Docker
- Build and run the docker-compose file
```bash
docker-compose -f Resources/docker/docker-compose.yaml up --build
```
- stop the containers
```bash
docker-compose -f Resources/docker/docker-compose.yaml down
```

- If you wish to run docker manually instead of through docker-compose, or wish to push the docker image to Docker Hub, follow the instructions in [ManualDockerInstructions.md](Resources/Docs/ManualDockerInstructions.md)

# Technical Info
## Architecture

This service uses a Layered Architecture pattern:

### API Layer
- Exposes endpoints
- Handles request/response mapping
### Business Logic Layer
- Application workflows
- Validation and business rules
### Data Access Layer
- Database interactions
- Repository implementations
- Persistence concerns
### Architecture Note

A Clean Architecture approach is also used in other services (e.g. User Service), but this service intentionally uses a Layered Architecture to reduce complexity for a smaller bounded context.

### Technical Stack
- Entity Framework Core (ORM)
    - Change tracking
    - LINQ support
    - Migrations (Code-First)
- Dependency Injection
- AutoMapper
- Fluent Validation (manual validation for Minimal APIs)
- Exception Handling Middleware
- Swagger / OpenAPI
- MySQL database
- Minimal API endpoints (lightweight alternative to MVC controllers)

### Design Notes
- Minimal APIs are used instead of Controllers
- FluentValidation is manually triggered due to lack of MVC pipeline integration
- EF Core is preferred over Dapper for maintainability and migration support in this service