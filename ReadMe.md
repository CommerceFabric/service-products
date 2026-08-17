# Products Microservice

## Technical Info

### Technical Stack

* MySQL
* Entity Framework Core (ORM)

  * Change tracking
  * LINQ support
  * Code-First migrations
* Dependency Injection
* AutoMapper
* FluentValidation (manual validation for Minimal APIs)
* Exception Handling Middleware
* Swagger / OpenAPI
* Minimal API endpoints
* Azure Service Bus (to consume order created events to lower stock)
* RabbitMQ (to publish product update/delete events to notify other services eg for orders service to update its cache)
* GitHub actions for CI, pushing docker images to the Azure Container Registry, then triggering [Infra-Platform](https://github.com/CommerceFabric/infra-platform/blob/main/docs/DeploymentFlow.md#microservice-release-sequence) to do the CD of deploying the new image to the AKS cluster.

### Architecture

This service uses a **Layered Architecture** pattern:

#### API Layer

* Exposes endpoints
* Handles request/response mapping

#### Business Logic Layer

* Application workflows
* Validation and business rules

#### Data Access Layer

* Database interactions
* Repository implementations
* Persistence concerns

> **Architecture Note**
> A Clean Architecture approach is also used in other services (e.g. User Service), but this service intentionally uses a Layered Architecture to reduce complexity for a smaller bounded context.

### Design Notes

* Minimal APIs are used instead of MVC Controllers.
* FluentValidation is manually triggered due to the lack of MVC pipeline integration in Minimal APIs.
* Entity Framework Core is preferred over Dapper for maintainability and migration support within this service.