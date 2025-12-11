
# 🏀 Basketball Analytics API

A production-ready REST API for basketball scouting and player analytics, built with **Clean Architecture**, **CQRS**, and modern .NET performance practices.

---

## 🛠 Tech Stack

| Category | Technology |
|----------|------------|
| **Framework** | .NET 8 (Latest LTS) |
| **Database** | PostgreSQL with EF Core |
| **Architecture** | Clean Architecture, CQRS |
| **Messaging** | MediatR with Pipeline Behaviors |
| **Validation** | FluentValidation |
| **Authentication** | JWT Bearer Tokens |
| **Caching** | In-Memory Cache |
| **Testing** | xUnit, Moq, Testcontainers |
| **Containerization** | Docker, Docker Compose |

---

## 🚀 Quick Start with Docker

The fastest way to run the application:

```bash
# Clone the repository
https://github.com/alkosmas/BasketballAnalytics-API.git
cd BasketballAnalytics.Api

# Run with Docker Compose (API + PostgreSQL)
docker-compose up

# API available at: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
# Health Check: http://localhost:5000/health
```

That's it! No need to install PostgreSQL or configure anything.

---

## 🏗 Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      CLEAN ARCHITECTURE                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐         │
│  │Presentation │───>│ Application │───>│   Domain    │         │
│  │   (API)     │    │  (Handlers) │    │ (Entities)  │         │
│  └─────────────┘    └─────────────┘    └─────────────┘         │
│         │                  │                                    │
│         │                  │                                    │
│         ▼                  ▼                                    │
│  ┌─────────────────────────────────────────────────┐           │
│  │            Infrastructure (Persistence)          │           │
│  └─────────────────────────────────────────────────┘           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Key Patterns

| Pattern | Implementation | Purpose |
|---------|---------------|---------|
| **CQRS** | Commands/Queries separation | Optimize reads vs writes |
| **MediatR** | Pipeline behaviors | Decoupling, cross-cutting concerns |
| **Repository** | DbContext as UoW | Data access abstraction |
| **Dependency Injection** | Built-in DI | Loose coupling, testability |

---

## ⚡ Performance Optimizations

### Database Level

| Optimization | Implementation | Benefit |
|--------------|----------------|---------|
| **AsNoTracking** | All read queries | Reduced memory, faster queries |
| **Projection** | Select only needed columns | Less data transfer |
| **Indexes** | Foreign keys indexed | O(log n) lookups |
| **Pagination** | Skip/Take with ordering | Handle large datasets |

### Application Level

| Optimization | Implementation | Benefit |
|--------------|----------------|---------|
| **Memory Caching** | 5-minute expiration | Reduce DB calls |
| **Cache Invalidation** | On Create/Update/Delete | Data consistency |
| **Async/Await** | All I/O operations | Non-blocking, scalability |
| **CancellationToken** | Passed to all async methods | Resource cleanup |

### Example: Optimized Query

```csharp
var players = await _context.Players
    .AsNoTracking()                    // No tracking overhead
    .Where(p => p.TeamId == teamId)    // Uses index
    .Select(p => new PlayerDto         // Projection - only needed columns
    {
        Id = p.Id,
        FullName = p.FirstName + " " + p.LastName,
        TeamName = p.Team.Name         // Auto-join, no N+1
    })
    .Skip((page - 1) * pageSize)       // Pagination
    .Take(pageSize)
    .ToListAsync(cancellationToken);   // Async + cancellation support
```

---

## ✨ Features

### Core Features
- ✅ Team management (CRUD)
- ✅ Player management with team relationships
- ✅ Team statistics with aggregations
- ✅ Paginated responses
- ✅ JWT authentication & authorization

### Data Integrity
- ✅ Request validation with FluentValidation
- ✅ Global error handling middleware
- ✅ Audit fields (CreatedAt, UpdatedAt)
- ✅ Restrict delete (protect related data)

### DevOps
- ✅ Docker & Docker Compose
- ✅ Health check endpoint
- ✅ Integration tests with Testcontainers

---

## 🔧 Local Development (without Docker)

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Docker (for integration tests)

### Configuration

Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=basketball;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "BasketballAnalytics",
    "Audience": "BasketballAnalytics",
    "ExpiryMinutes": 60
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
cd ../../..

# Run the API
dotnet run --project src/Presentation/BasketballAnalytics.Api
```

### Run Tests

```bash
# All tests
dotnet test

# Specific test project
dotnet test tests/BasketballAnalytics.Api.Tests/
```

---

## 📡 API Endpoints

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Check API and database status |

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get JWT token |

### Teams

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/teams` | Get all teams (cached) |
| GET | `/api/teams/{id}` | Get team by ID |
| GET | `/api/teams/{id}/stats` | Get team statistics |
| POST | `/api/teams` | Create new team |
| PUT | `/api/teams/{id}` | Update team |
| DELETE | `/api/teams/{id}` | Delete team (Admin only) |

### Players

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/players?page=1&pageSize=20` | Get players (paginated) |
| GET | `/api/players/team/{teamId}` | Get players by team |
| POST | `/api/players` | Create new player |

---

## 📁 Project Structure

```
BasketballAnalytics.Api/
├── src/
│   ├── Core/
│   │   ├── Domain/                 # Entities, Enums (zero dependencies)
│   │   │   └── Entities/
│   │   │       ├── BaseEntity.cs   # Audit fields
│   │   │       ├── Team.cs
│   │   │       └── Player.cs
│   │   └── Application/            # Business logic
│   │       ├── Common/
│   │       │   ├── Behaviors/      # Validation, Logging pipelines
│   │       │   ├── Interfaces/     # Abstractions
│   │       │   └── Models/         # Pagination, etc.
│   │       └── Features/
│   │           ├── Teams/          # Team commands & queries
│   │           └── Players/        # Player commands & queries
│   ├── Infrastructure/
│   │   └── Persistence/            # EF Core implementation
│   │       ├── DbContext/
│   │       └── Migrations/
│   └── Presentation/
│       └── Api/                    # HTTP layer
│           ├── Controllers/
│           ├── Middleware/         # Error handling
│           └── Dockerfile
└── tests/
    ├── BasketballAnalytics.Api.Tests/
    ├── BasketballAnalytics.Application.Tests/
    ├── BasketballAnalytics.IntegrationTests/
    └── BasketballAnalytics.Tests.Common/
├── docker-compose.yml
└── README.md
```

---

## 🧪 Testing Strategy

| Type | Tools | Purpose |
|------|-------|---------|
| **Unit Tests** | xUnit, Moq | Test handlers in isolation |
| **Integration Tests** | Testcontainers | Test with real PostgreSQL |

---

## 📈 Future Enhancements

- [ ] GamePerformance entity for player statistics
- [ ] Advanced player analytics (PPG, RPG, APG)
- [ ] Player comparison endpoints
- [ ] Background jobs for stats aggregation
- [ ] Redis distributed caching
- [ ] Rate limiting

---

## 👤 Author

**Alexandros Kosmas**
- GitHub: [@alkosmas92](https://github.com/alkosmas92)
- LinkedIn: [alexandros-kosmas](https://www.linkedin.com/in/alexandros-kosmas)

---

## 📄 License

This project is for educational and portfolio purposes.
EOF
```

---

## Build και commit:

```bash
dotnet build
git add .
git commit -m "Update README with Docker instructions and complete documentation"
```

---
