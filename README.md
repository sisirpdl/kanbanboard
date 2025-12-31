# Kanban Board

A Kanban Board application built with ASP.NET Core and Clean Architecture principles.

## About

This project is based on [Ardalis' Clean Architecture template](https://github.com/ardalis/CleanArchitecture) by Steve Smith (@ardalis). The core architecture and patterns come from that template - I've added Kanban Board functionality on top of it.

## Features

- Create, update, and delete tasks
- Drag and drop tasks between columns
- Multiple board support with custom board names
- Five status columns: Todo, In Progress, In Review, Done, Archived
- Clean separation of concerns following DDD principles

## Tech Stack

- .NET 10.0
- FastEndpoints (REPR pattern)
- Entity Framework Core with SQLite
- Ardalis.SmartEnum for type-safe enums
- Ardalis.Specification for repository pattern
- Scalar for API docs
- Sortable.js for drag-drop
- Vanilla JavaScript frontend

## Project Structure

```
KanbanBoard.Core          - Domain entities, events, specifications
KanbanBoard.UseCases      - Commands and queries (CQRS)
KanbanBoard.Infrastructure - Data access, migrations
KanbanBoard.Web           - API endpoints, static files
```

## Getting Started

Clone and run:

```bash
git clone https://github.com/sisirpdl/kanbanboard.git
cd kanbanboard
dotnet run --project src/KanbanBoard.Web
```

Open https://localhost:57679/ in your browser.

The app uses SQLite - the database file gets created automatically at `src/KanbanBoard.Infrastructure/kanban.db`.

## API Documentation

Interactive API docs available at https://localhost:57679/scalar/v1

Endpoints:
- `GET /tasks/{boardId}` - Get tasks for a board
- `POST /tasks` - Create task
- `PUT /tasks/{id}` - Update task
- `PATCH /tasks/{id}/move` - Move task
- `DELETE /tasks/{id}` - Delete task
- `POST /boards` - Create board

## Architecture

The domain model includes:
- **KanbanTask** - Task aggregate root
- **TaskStatus** - SmartEnum (Todo, InProgress, InReview, Done, Archived)
- **TaskMovedEvent** - Domain event for tracking movements
- **Specifications** - TasksByBoardSpec, TaskByIdSpec

Commands and queries are separated following CQRS. Using Vogen for strongly-typed IDs.

## Migrations

Add migration:
```powershell
dotnet ef migrations add MigrationName -c AppDbContext -p src/KanbanBoard.Infrastructure/KanbanBoard.Infrastructure.csproj -s src/KanbanBoard.Web/KanbanBoard.Web.csproj -o Data/Migrations
```

Update database:
```powershell
dotnet ef database update -c AppDbContext -p src/KanbanBoard.Infrastructure/KanbanBoard.Infrastructure.csproj -s src/KanbanBoard.Web/KanbanBoard.Web.csproj
```

## Credits

Built on the [Clean Architecture template](https://github.com/ardalis/CleanArchitecture) by Steve Smith. The foundational architecture and infrastructure code come from that template.

If you want to learn more about Clean Architecture:
- [Clean Architecture Template](https://github.com/ardalis/CleanArchitecture)
- [NimblePros Clean Architecture Course](https://academy.nimblepros.com/p/learn-clean-architecture)
- [DDD Fundamentals](https://www.pluralsight.com/courses/fundamentals-domain-driven-design)

## License

MIT
