# 🏀 Basketball Analytics API

A production-ready REST API for basketball scouting and player analytics, built with **Clean Architecture** and modern .NET practices.

## 🛠 Tech Stack

- **.NET 8** - Latest LTS version
- **Entity Framework Core** - ORM with PostgreSQL
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **JWT Authentication** - Secure API access
- **xUnit + Testcontainers** - Integration testing with real database

## 🏗 Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
src/
├── Core/
│   ├── Domain/           # Entities, Enums (no dependencies)
│   └── Application/      # Business logic, CQRS handlers, DTOs
├── Infrastructure/
│   └── Persistence/      # EF Core, Database configurations
└── Presentation/
    └── Api/              # Controllers, Middleware
```

### Key Patterns Used

| Pattern | Implementation |
|---------|---------------|
| **CQRS** | Separate Commands and Queries via MediatR |
| **Repository** | DbContext as Unit of Work |
| **Pipeline Behaviors** | Validation, Logging via MediatR |
| **Dependency Injection** | Built-in .NET DI container |

## ✨ Features

- ✅ Team management (CRUD)
- ✅ Player management with team relationships
- ✅ Pagination support
- ✅ Memory caching with cache invalidation
- ✅ JWT authentication
- ✅ Global error handling
- ✅ Request validation
- ✅ Audit fields (CreatedAt, UpdatedAt)

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Docker (for integration tests)

### Configuration

Update `appsettings.json` with your database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=basketball;Username=postgres;Password=yourpassword"
  }
}
```

### Run the Application

```bash
# Restore dependencies
dotnet restore

# Apply migrations
cd src/Infrastructure/BasketballAnalytics.Persistence
dotnet ef database update --startup-project ../../Presentation/BasketballAnalytics.Api

# Run the API
cd ../../Presentation/BasketballAnalytics.Api
dotnet run
```

The API will be available at `https://localhost:5001`

### Run Tests

```bash
dotnet test
```

## 📡 API Endpoints

### Teams

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/teams` | Get all teams |
| GET | `/api/teams/{id}` | Get team by ID |
| POST | `/api/teams` | Create new team |
| PUT | `/api/teams/{id}` | Update team |
| DELETE | `/api/teams/{id}` | Delete team |

### Players

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/players` | Get all players (paginated) |
| GET | `/api/players/team/{teamId}` | Get players by team |
| POST | `/api/players` | Create new player |

## 📁 Project Structure

```
BasketballAnalytics/
├── src/
│   ├── Core/
│   │   ├── BasketballAnalytics.Application/
│   │   │   ├── Common/
│   │   │   │   ├── Behaviors/
│   │   │   │   ├── Interfaces/
│   │   │   │   └── Models/
│   │   │   └── Features/
│   │   │       ├── Teams/
│   │   │       └── Players/
│   │   └── BasketballAnalytics.Domain/
│   │       └── Entities/
│   │           ├── BaseEntity.cs
│   │           ├── Team.cs
│   │           └── Player.cs
│   ├── Infrastructure/
│   │   └── BasketballAnalytics.Persistence/
│   │       ├── Authentication/
│   │       ├── DbContext/
│   │       └── Migrations/
│   └── Presentation/
│       └── BasketballAnalytics.Api/
│           ├── Controllers/
│           ├── Middleware/
│           ├── Properties/
│           ├── appsettings.json
│           ├── appsettings.Development.json
│           └── Program.cs
└── tests/
    ├── BasketballAnalytics.Api.Tests/
    ├── BasketballAnalytics.Application.Tests/
    ├── BasketballAnalytics.IntegrationTests/
    └── BasketballAnalytics.Tests.Common/
```

## 📄 License

This project is for educational and portfolio purposes.