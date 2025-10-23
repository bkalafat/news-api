# News API - GitHub Copilot Instructions

This file provides comprehensive context for GitHub Copilot to assist effectively with the News API project.

## 📋 Project Overview

**News API** is a modern news management system built with .NET 10 that provides a RESTful API for managing news articles with features including JWT authentication, MongoDB persistence, and comprehensive caching.

**Target Audience**: 
- Frontend developers building news consumption applications
- Backend developers maintaining and extending the API
- DevOps engineers deploying and monitoring the service

**Key Features**:
- JWT-based authentication for protected endpoints
- Clean Architecture with clear separation of concerns
- MongoDB for flexible NoSQL storage
- Memory caching for high-performance read operations
- Comprehensive input validation with FluentValidation
- OWASP Top 10 security protections
- Health monitoring and Swagger documentation

## 🛠️ Tech Stack

### Backend Framework
- **.NET 10.0** - Latest LTS version with C# 12+ features
- **ASP.NET Core** - Web API framework with minimal APIs support
- **Clean Architecture** - Domain, Application, Infrastructure, Presentation layers

### Database & Caching
- **MongoDB 2.30+** - Primary data store with MongoDB.Driver
- **IMemoryCache** - Built-in ASP.NET Core memory caching
- **Separate databases** - Dev, staging, and production environments

### Authentication & Security
- **JWT Bearer Tokens** - OAuth2/OpenID Connect compliant authentication
- **User Secrets** - Development secret management
- **Azure Key Vault** - Production secret management (ready)
- **FluentValidation** - Server-side input validation
- **Security Middleware** - HSTS, CSP, X-Frame-Options headers

### Testing
- **xUnit** - Primary testing framework
- **Moq** - Mocking framework for unit tests
- **WebApplicationFactory** - Integration testing
- **BenchmarkDotNet** - Performance benchmarking (if needed)

### Documentation & DevOps
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation
- **Docker** - Containerization with multi-stage builds
- **Heroku** - Cloud deployment platform
- **Git** - Version control with feature branching

## 📁 Project Structure

```
news-api/
├── backend/                          # Main API project
│   ├── Domain/                       # Core business entities (no external dependencies)
│   │   ├── Entities/                # News entity models
│   │   └── Interfaces/              # Repository contracts (INewsRepository)
│   ├── Application/                 # Business logic & use cases
│   │   ├── DTOs/                    # Data Transfer Objects (CreateNewsDto, UpdateNewsDto)
│   │   ├── Services/                # Business services (NewsService, INewsService)
│   │   └── Validators/              # FluentValidation rules (NewsValidator)
│   ├── Infrastructure/              # External dependencies & implementations
│   │   ├── Data/                    # MongoDB context, repositories, models, mappers
│   │   ├── Caching/                 # Memory cache service
│   │   ├── Security/                # JWT token service
│   │   ├── Services/                # External service integrations
│   │   └── HealthChecks/            # MongoDB health check
│   ├── Presentation/                # API layer & HTTP concerns
│   │   ├── Controllers/             # API endpoints (NewsController)
│   │   ├── Middleware/              # Security & validation middleware
│   │   └── Extensions/              # Service collection extensions
│   ├── Common/                      # Shared constants (CacheKeys)
│   ├── Properties/                  # Launch settings and publish profiles
│   └── Program.cs                   # Application entry point
├── tests/                           # Test suite
│   ├── Unit/                        # Unit tests for services, validators, DTOs
│   ├── Integration/                 # Controller & repository integration tests
│   ├── Performance/                 # Performance benchmarks
│   └── Helpers/                     # Test utilities (TestDataBuilders, TestMemoryCache)
├── frontend/                        # Next.js 15 frontend
├── docker/                          # Docker configurations
├── scripts/                         # Development scripts
│   └── database/                    # Data migration scripts
├── .github/                         # GitHub configuration
│   ├── instructions/                # Copilot instruction files
│   ├── prompts/                     # Reusable prompt files
│   └── cursor-rules.md              # Cursor IDE rules
└── README.md                        # Project documentation

```

**Important Folders**:
- `Domain/` - Pure business logic, no framework dependencies
- `Application/` - Use cases and business rules
- `Infrastructure/` - Data access, caching, external services
- `Presentation/` - Controllers, middleware, API contracts
- `tests/` - Comprehensive test coverage

## 🎯 Project & Code Guidelines

### General Principles
- **Follow Clean Architecture** - Dependencies always point inward (Presentation → Application → Domain)
- **Use SOLID principles** - Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
- **Dependency Injection** - All services registered in ServiceCollectionExtensions
- **Async/Await** - All I/O operations must be asynchronous
- **Nullable Reference Types** - Enabled globally with `#nullable enable`

### C# Coding Standards
- **Follow Microsoft C# Conventions** - [Official Guidelines](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Use PascalCase** - For classes, methods, properties, constants
- **Use camelCase** - For local variables, parameters, private fields
- **Use `_camelCase`** - For private fields (with underscore prefix)
- **Use type inference** - `var` for obvious types, explicit for clarity
- **Use expression-bodied members** - When appropriate for single-line methods
- **Use pattern matching** - C# 12+ switch expressions and patterns
- **Use records** - For immutable DTOs when appropriate

### API Development
- **Use `[ApiController]` attribute** - Enables automatic model validation
- **Use `ActionResult<T>`** - For type-safe responses
- **Use `ProducesResponseType`** - Document all possible response codes
- **RESTful conventions** - GET (read), POST (create), PUT (update), DELETE (delete)
- **Status codes** - 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 404 Not Found, 500 Internal Server Error
- **Problem Details** - Use RFC 7807 format for errors
- **Versioning ready** - Structure supports future API versioning

### Validation Rules
- **Use FluentValidation** - All DTOs must have validators
- **Server-side validation** - Never trust client input
- **Required fields** - Explicitly validate required fields
- **Max lengths** - Enforce string length constraints
- **Format validation** - URLs, emails, dates must be validated
- **Business rules** - Validate in service layer, not controllers

### Database & Data Access
- **Repository Pattern** - All data access through INewsRepository
- **No EF Core** - MongoDB with official driver only
- **Async queries** - All database operations asynchronous
- **Connection strings** - Never hardcode, use User Secrets or Key Vault
- **Projections** - Only query fields you need
- **Indexes** - Ensure proper MongoDB indexes for performance

### Security Guidelines
- **No secrets in code** - Use User Secrets (dev) or Azure Key Vault (prod)
- **Validate all inputs** - Server-side validation is mandatory
- **Use parameterized queries** - Prevent injection attacks
- **Implement HTTPS** - Always in production
- **Security headers** - HSTS, X-Content-Type-Options, X-Frame-Options, CSP
- **JWT best practices** - Secure secret key (min 32 chars), reasonable expiration
- **CORS policy** - Whitelist specific origins only

### Testing Requirements
- **Write tests first or alongside code** - TDD or test-alongside development
- **Use Arrange-Act-Assert pattern** - Clear test structure
- **Test public interfaces only** - Don't test private methods directly
- **Mock external dependencies** - Use Moq for services, databases
- **Integration tests** - Use WebApplicationFactory for API tests
- **Test edge cases** - Null values, empty strings, boundary conditions
- **Test error paths** - Exceptions, validation failures
- **Naming convention** - `MethodName_Scenario_ExpectedBehavior`

### Documentation
- **XML comments** - For all public APIs (classes, methods, properties)
- **Summary tags** - Brief description of purpose
- **Param tags** - Document parameters with meaningful descriptions
- **Returns tags** - Describe return values
- **Example tags** - Include usage examples for complex APIs
- **Swagger annotations** - Use `[ProducesResponseType]`, `[SwaggerOperation]` when needed

### Error Handling
- **Use middleware** - ValidationMiddleware for request validation
- **Log errors** - Use ILogger<T> for structured logging
- **Don't expose internals** - Generic error messages to clients
- **Use try-catch** - Only for expected errors
- **Use Result pattern** - For predictable failure scenarios
- **Problem Details** - Return structured error responses

### Performance
- **Enable caching** - Use IMemoryCache for frequently accessed data
- **Cache expiration** - 30 minutes default, configurable
- **Async I/O** - Never block threads
- **Pagination** - For large result sets (to be implemented)
- **Lazy loading** - Load related data only when needed
- **Compression** - Enable response compression

## 🔧 Resources & Scripts

### Available Scripts
Located in project root or can be run via VS Code tasks:

- **Build & Run**
  ```bash
  dotnet build
  dotnet run --project backend/newsApi.csproj
  ```

- **Testing**
  ```bash
  dotnet test                           # Run all tests
  dotnet test --filter "FullyQualifiedName~Unit"  # Unit tests only
  dotnet test --filter "FullyQualifiedName~Integration"  # Integration tests only
  ```

- **Database Management**
  - See `scripts/database/data-migration.md` for migration scripts
  - MongoDB must be running locally or provide connection string

- **Docker**
  ```bash
  docker build -t news-api:latest .
  docker run -p 5000:8080 news-api:latest
  ```

### Configuration Management
- **Development**: `dotnet user-secrets set "Key" "Value"`
- **Testing**: `appsettings.Testing.json` with test-specific values
- **Production**: Environment variables or Azure Key Vault

### Health Checks
- **Endpoint**: `GET /health`
- **Monitors**: MongoDB connection status
- **Returns**: "Healthy" (200) or error details (503)

### Swagger/OpenAPI
- **URL**: `http://localhost:5000/swagger`
- **Authentication**: Click "Authorize" button, enter `Bearer {token}`
- **Try It Out**: Enabled by default for testing

## 🧩 Common Development Tasks

### Adding a New API Endpoint
1. Define DTO in `Application/DTOs/`
2. Add validation in `Application/Validators/`
3. Implement service method in `Application/Services/NewsService.cs`
4. Add controller action in `Presentation/Controllers/NewsController.cs`
5. Add XML documentation comments
6. Write unit tests in `tests/Unit/`
7. Write integration tests in `tests/Integration/`

### Adding a New Entity
1. Create entity in `Domain/Entities/`
2. Define repository interface in `Domain/Interfaces/`
3. Create MongoDB model in `Infrastructure/Data/Models/`
4. Create mapper in `Infrastructure/Data/Mappers/`
5. Implement repository in `Infrastructure/Data/Repositories/`
6. Register in DI container in `Presentation/Extensions/ServiceCollectionExtensions.cs`

### Adding Validation Rules
1. Create or update validator in `Application/Validators/`
2. Inherit from `AbstractValidator<TDto>`
3. Define rules in constructor using FluentValidation syntax
4. Register in DI container (auto-registered with AddValidatorsFromAssemblyContaining)
5. Write validator tests in `tests/Unit/Validators/`

### Debugging Tips
- Set breakpoints in controllers and services
- Use Swagger UI for interactive API testing
- Check `appsettings.Development.json` for logging levels
- Use `dotnet watch run` for hot reload during development
- Check MongoDB Compass for database state

## 🚫 Patterns to Avoid

- **Don't use static state** - Except for constants
- **Don't use service locator** - Always use constructor injection
- **Don't put logic in controllers** - Keep controllers thin, delegate to services
- **Don't expose domain entities** - Use DTOs for API responses
- **Don't hardcode configuration** - Use appsettings.json and User Secrets
- **Don't use EF Core** - This is a MongoDB project
- **Don't ignore validation** - Always validate user input
- **Don't catch generic exceptions** - Be specific with exception handling
- **Don't use async void** - Use async Task instead
- **Don't block on async code** - Avoid .Result or .Wait()

## 📚 Additional References

### Official Documentation
- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/current/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [xUnit](https://xunit.net/)

### Project Documentation
- `NEWS_API_DOCUMENTATION.md` - Complete API reference
- `SWAGGER_TESTING_GUIDE.md` - Interactive testing guide
- `specs/002-modernize-net-core/spec.md` - Architecture specifications
- `scripts/database/data-migration.md` - Data migration guide
- `tests/TEST_COVERAGE_REPORT.md` - Test coverage details

### Architecture Resources
- [Clean Architecture by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [Microsoft Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/)

## 💡 Tips for Effective Copilot Usage

1. **Provide context** - Include entity names, layer names, and purpose in prompts
2. **Be specific** - "Add a validator for CreateNewsDto with max length 500 for Caption"
3. **Reference existing patterns** - "Following the pattern in NewsService, add a new method..."
4. **Specify layer** - "In the Application layer, create..." or "Add to NewsController..."
5. **Include validation** - Always ask for validation rules when creating DTOs
6. **Request tests** - "...and write unit tests for the new method"
7. **Iterate** - Review generated code and refine with follow-up prompts

## 🔄 Iteration & Review

- **Review all generated code** - Copilot suggestions should be reviewed before committing
- **Run tests after changes** - Ensure `dotnet test` passes
- **Check code style** - Use Visual Studio or Rider code inspections
- **Update documentation** - Keep XML comments and README in sync
- **Follow PR process** - Create feature branches, write meaningful commit messages

---

**Last Updated**: October 2025  
**Copilot Version Compatibility**: GitHub Copilot with VS Code or Visual Studio 2022+
