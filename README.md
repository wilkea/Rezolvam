# Rezolvam

Rezolvam is a simple reporting application built with **ASP.NET Core 8** using a clean architecture approach. Users can submit reports with photos and comments while administrators manage the report lifecycle.

## Features

- ASP.NET Core MVC application for public users and administrators (AdminMVC project)
- Separate Web API project (`rezolvam.Api`) prepared for REST endpoints
- Application layer with MediatR commands and queries
- Domain layer containing entities such as reports, comments and photos
- Infrastructure layer using Entity Framework Core with SQL Server
- Authentication and authorization with ASP.NET Core Identity and JWT

## Repository Structure

- `AdminMVC/` – MVC application with views and controllers
- `rezolvam.Api/` – API project (currently minimal)
- `rezolvam.Application/` – application logic (CQRS)
- `rezolvam.Domain/` – domain entities and rules
- `rezolvam.Infrastructure/` – data access and identity implementation
- `rezolvam.sln` – Visual Studio solution file

## Requirements

- .NET 8 SDK
- SQL Server (LocalDB by default)

## Getting Started

1. Restore packages and build the solution:
   ```bash
   dotnet build rezolvam.sln
   ```
2. Apply the database migrations:
   ```bash
   dotnet ef database update --project rezolvam.Infrastructure --startup-project AdminMVC
   ```
3. Run the MVC application:
   ```bash
   dotnet run --project AdminMVC
   ```

Connection strings and JWT settings are found in `appsettings.json` inside each project. Adjust them as needed for your environment.

## Documentation

Additional documentation is available in the [`docs`](docs/) folder.

