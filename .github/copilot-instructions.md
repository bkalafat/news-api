# news-api Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-09-27

## Active Technologies
- .NET 8 LTS with C# 12 features and nullable reference types
- ASP.NET Core 8.0 Web API framework with minimal hosting model
- MongoDB with MongoDB.Driver 2.28+ for document database operations
- FluentValidation 11.3+ for comprehensive input validation
- JWT Bearer Authentication for secure API access
- Clean Architecture with Domain, Application, Infrastructure, and Presentation layers

## Project Structure
```
newsApi/
├── Domain/                 # Core business entities and interfaces
├── Application/           # Business logic, services, DTOs, validators  
├── Infrastructure/        # Data access, external services, caching
├── Presentation/         # Controllers, middleware, extensions
├── Common/              # Shared utilities and constants
├── Program.cs           # Application entry point (.NET 8 minimal hosting)
└── newsApi.csproj      # Project file targeting .NET 8
```

## Commands
```bash
# .NET 8 Development Commands
dotnet restore                    # Restore NuGet packages
dotnet build                     # Build the project
dotnet run                       # Run the application
dotnet user-secrets set "key" "value"  # Configure development secrets
dotnet ef database update       # Apply database migrations
dotnet test                     # Run tests

# MongoDB Commands
mongosh                         # MongoDB shell
db.News.find()                 # Query news collection
```

## Code Style
**.NET 8 with C# 12**: Follow official Microsoft conventions with modern C# features
- Use nullable reference types throughout
- Leverage record types for DTOs
- Apply async/await patterns consistently
- Follow Clean Architecture dependency rules
- Use dependency injection for all services
- Implement FluentValidation for input validation

## Recent Changes
- 002-modernize-net-core: Upgraded from .NET Core 3.1 to .NET 8 LTS with Clean Architecture implementation

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
