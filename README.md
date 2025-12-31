# Kanban Board

A Clean Architecture-based Kanban Board application built with ASP.NET Core and FastEndpoints.

## About This Project

This project is built on top of [Ardalis' Clean Architecture template](https://github.com/ardalis/CleanArchitecture). The foundational architecture, patterns, and project structure are derived from that template, with custom Kanban Board functionality implemented on top.

**Original Template:** [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
**Template Author:** Steve Smith (@ardalis)
**License:** MIT

## Features

- **Task Management**: Create, update, and delete tasks across different status columns
- **Drag & Drop**: Intuitive drag-and-drop interface powered by Sortable.js
- **Multiple Boards**: Support for multiple independent Kanban boards with localStorage-based board names
- **Task Statuses**: Todo, In Progress, In Review, Done, and Archived columns
- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **Modern UI**: Enterprise-grade, professional interface with responsive design

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **FastEndpoints** - REPR pattern for API endpoints
- **Entity Framework Core** - Data access with SQLite
- **Ardalis.SmartEnum** - Type-safe enumerations for task status
- **Ardalis.Specification** - Repository pattern with specifications
- **Scalar** - Interactive API documentation
- **Sortable.js** - Drag-and-drop functionality
- **Vanilla JavaScript** - No heavy frontend frameworks

## Project Structure

Following Clean Architecture principles:

- **KanbanBoard.Core** - Domain entities, value objects, domain events, specifications
- **KanbanBoard.UseCases** - Application logic, commands, and queries (CQRS pattern)
- **KanbanBoard.Infrastructure** - Data access, EF Core migrations, external services
- **KanbanBoard.Web** - API endpoints, static files, frontend UI

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Any code editor (Visual Studio, VS Code, Rider)

### Running the Application

1. Clone the repository:
```bash
git clone https://github.com/sisirpdl/kanbanboard.git
cd kanbanboard
```

2. Run the application:
```bash
dotnet run --project src/KanbanBoard.Web
```

3. Open your browser to:
```
https://localhost:57679/
```

The application will automatically redirect to the Kanban Board interface.

### Database

This application uses SQLite for local development. The database file is automatically created at:
```
src/KanbanBoard.Infrastructure/kanban.db
```

Migrations are already applied during startup. No manual database setup required.

## API Documentation

Access the interactive API documentation via Scalar at:
```
https://localhost:57679/scalar/v1
```

### Available Endpoints

- `GET /tasks/{boardId}` - Get all tasks for a board
- `POST /tasks` - Create a new task
- `PUT /tasks/{id}` - Update an existing task
- `PATCH /tasks/{id}/move` - Move task to different status/position
- `DELETE /tasks/{id}` - Delete a task
- `POST /boards` - Create a new board

## Architecture Highlights

### Domain Model

The core domain includes:
- **KanbanTask** aggregate - Main task entity with business logic
- **TaskStatus** SmartEnum - Type-safe status enumeration (Todo, InProgress, InReview, Done, Archived)
- **Domain Events** - `TaskMovedEvent` for tracking task movements
- **Specifications** - `TasksByBoardSpec`, `TaskByIdSpec` for querying

### CQRS Pattern

Commands and queries are separated:
- **Commands**: CreateTask, UpdateTask, MoveTask, DeleteTask
- **Queries**: GetTasksByBoard, GetTaskById

### Value Objects

Using Vogen for strongly-typed IDs and value objects to prevent primitive obsession.

## Clean Architecture Resources

To learn more about the Clean Architecture pattern this project is based on:

- [Clean Architecture Template](https://github.com/ardalis/CleanArchitecture)
- [NimblePros Clean Architecture Course](https://academy.nimblepros.com/p/learn-clean-architecture)
- [DDD Fundamentals by Steve Smith](https://www.pluralsight.com/courses/fundamentals-domain-driven-design)

## Database Migrations

To add a new migration:
```powershell
dotnet ef migrations add MigrationName -c AppDbContext -p src/KanbanBoard.Infrastructure/KanbanBoard.Infrastructure.csproj -s src/KanbanBoard.Web/KanbanBoard.Web.csproj -o Data/Migrations
```

To update the database:
```powershell
dotnet ef database update -c AppDbContext -p src/KanbanBoard.Infrastructure/KanbanBoard.Infrastructure.csproj -s src/KanbanBoard.Web/KanbanBoard.Web.csproj
```

## Acknowledgments

This project is built upon the excellent [Clean Architecture template](https://github.com/ardalis/CleanArchitecture) by Steve "ardalis" Smith. The architecture, patterns, and much of the infrastructure code come from that template. Custom Kanban Board business logic and UI have been implemented on top of this foundation.

## License

MIT
