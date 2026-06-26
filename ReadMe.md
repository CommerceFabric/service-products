# Todo

- Seed the DB using a startup seeder
- Generate migrations for the database using EF Core (Code-First approach using migrations)

---

# Requirements

- [MySQL](https://dev.mysql.com/downloads/mysql/) Community Server 8.0 or higher

---

## Initial Database Seed (temporary manual setup)

Run the following SQL to initialise and seed the database (this will later be replaced by EF Core migrations + startup seeder):

```sql
-- Create the database
CREATE DATABASE IF NOT EXISTS productService;
USE productService;

-- Create the products table
CREATE TABLE IF NOT EXISTS products (
  ProductID char(36) NOT NULL,
  ProductName varchar(50) NOT NULL,
  Category varchar(50) DEFAULT NULL,
  UnitPrice decimal(10,2) DEFAULT NULL,
  QuantityInStock int DEFAULT NULL,
  PRIMARY KEY (ProductID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Seed data
INSERT INTO products (ProductID, ProductName, Category, UnitPrice, QuantityInStock) VALUES
  ('1a9df78b-3f46-4c3d-9f2a-1b9f69292a77', 'Apple iPhone 15 Pro Max', 'Electronics', 1299.99, 50),
  ('2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17', 'Samsung Foldable Smart Phone 2', 'Electronics', 1499.99, 100),
  ('3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1', 'Ergonomic Office Chair', 'Furniture', 249.99, 25),
  ('4c9b6f71-6c5d-485f-8db2-58011a236b63', 'Coffee Table with Storage', 'Furniture', 179.99, 30),
  ('5d7e36bf-65c3-4a71-bf97-740d561d8b65', 'Samsung QLED 75 inch', 'Electronics', 1999.99, 20),
  ('6a14f510-72c1-42c8-9a5a-8ef8f3f45a0d', 'Running Shoes', 'Accessories', 49.99, 75),
  ('7b39ef14-932b-4c84-9187-55b748d2b28f', 'Anti-Theft Laptop Backpack', 'Accessories', 59.99, 60),
  ('8c5f6e73-68fc-49d9-99b4-aecc3706a4f4', 'LG OLED 65 inch', 'Electronics', 1499.99, 15),
  ('9e7e7085-6f4e-4921-8f15-c59f084080f9', 'Modern Dining Table', 'Furniture', 699.99, 10),
  ('10d7b110-ecdb-4921-85a4-58a5d1b32bf4', 'PlayStation 5', 'Electronics', 499.99, 40),
  ('11f2e86a-9d5d-42f9-b3c2-3e4d652e3df8', 'Executive Office Desk', 'Furniture', 299.99, 18),
  ('12b369b7-9101-41b1-a653-6c6c9a4fe1e4', 'Breville Smart Blender', 'HomeAppliances', 129.99, 50);
```

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
docker run -d --name mysql-productservice --network productsmicroservice-network -e MYSQL_ROOT_PASSWORD=admin -e MYSQL_DATABASE=productService mysql:9.7.1
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