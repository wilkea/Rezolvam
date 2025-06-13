# Project Architecture

The solution follows a layered architecture inspired by Clean Architecture.

```
rezolvam.sln
├── AdminMVC/            ASP.NET Core MVC web application
├── rezolvam.Api/        ASP.NET Core Web API project
├── rezolvam.Application/  Application layer (CQRS, services)
├── rezolvam.Domain/       Domain entities and business rules
└── rezolvam.Infrastructure/ Infrastructure (EF Core, Identity)
```

## Layers

- **Domain**: contains entity classes and domain logic, independent of infrastructure.
- **Application**: uses MediatR for commands and queries, orchestrating business rules.
- **Infrastructure**: implements persistence with Entity Framework Core, authentication, and file storage.
- **AdminMVC**: main user interface for submitting and managing reports.
- **rezolvam.Api**: API endpoints (currently minimal) that expose application features.

Each layer references only the layers below it to keep dependencies isolated.

## Database

The application uses SQL Server with Entity Framework Core. Migrations live under `rezolvam.Infrastructure/Migrations` and can be applied via `dotnet ef database update`.

## Authentication

ASP.NET Core Identity provides user and role management. JWT authentication is also configured and can be used for the API.

