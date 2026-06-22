# Requirements

- ...

# Technical Info

## Architecture
- Uses the Layered Architecture (API / BusinessLogic / DataAccess) pattern, which separates the application into presentation, business, and persistence layers.
    - API – Exposes endpoints and handles request/response mapping.
    - BusinessLogic – Contains application workflows, validation, and business rules.
    - DataAccess – Manages database interactions, repositories, and persistence concerns.

- In recent years, Clean Architecture has gained popularity for its emphasis on separation of concerns and dependency inversion, though these benefits come with additional complexity.
    - I implemented the [User Service](https://github.com/CommerceFabric/service-user) micro-service using this approach.
    - But for this micro-service, I intentionally chose the Layered Architecture pattern, as its lower complexity makes it well-suited to small and medium-sized applications.

## Technical Stack