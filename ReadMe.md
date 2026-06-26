# Todo

- Seed the DB using a startup seeder
- Generate migrations for the database using EF Core (Code-First approach using migrations)

---

# Requirements

- [MySQL](https://dev.mysql.com/downloads/mysql/) Community Server 8.0 or higher

---

## Initial Database Seed (temporary manual setup)

Run the following SQL to initialise and seed the database (this will later be replaced by EF Core migrations + startup seeder):
- [db_products_seed.sql](Resources\dbSeed.sql)

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
1. Create network
```bash
docker network create productsmicroservice-network
```
2. Run MySQL container
```bash
docker run -d --name mysql-productservice --network productsmicroservice-network -e MYSQL_ROOT_PASSWORD=admin -e MYSQL_DATABASE=productService -v ./Resources/mysql-init:/docker-entrypoint-initdb.d -p 3306:3306 mysql:9.7.1
```

Note: MySQL must NOT already be running locally on port 3306 unless you change the port mapping.

3. Build microservice image
```bash
docker build -t danielmusselwhite/commercefabric_product_microservice:1.0.0 -f .\CommerceFabric.ProductService\Dockerfile .
```

4. Run microservice
```bash
docker run -p 8080:8080 --network productsmicroservice-network danielmusselwhite/commercefabric_product_microservice:1.0.0
```

5. Push to Docker Hub
```bash
docker push danielmusselwhite/commercefabric_product_microservice:1.0.0
```

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