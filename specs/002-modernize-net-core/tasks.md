# Tasks: .NET 8 Modernization with Clean Architecture

**Input**: Design documents from `/specs/002-modernize-net-core/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Phase 3.1: Setup
- [x] T001 Update project file to target .NET 8 LTS with C# 12 features in newsApi/newsApi.csproj
- [x] T002 Upgrade NuGet packages to latest versions (MongoDB.Driver 2.28+, FluentValidation 11.3+, JWT Bearer Authentication) in newsApi/newsApi.csproj
- [x] T003 Create Clean Architecture folder structure (Domain, Application, Infrastructure, Presentation) in newsApi/

## Phase 3.2: Domain Layer Implementation
- [x] T004 [P] Create News entity with C# 12 nullable support in newsApi/Domain/Entities/News.cs
- [x] T005 [P] Create INewsRepository interface in newsApi/Domain/Interfaces/INewsRepository.cs
- [x] T006 [P] Move CacheKeys to Common folder in newsApi/Common/CacheKeys.cs

## Phase 3.3: Application Layer Implementation
- [x] T007 [P] Create INewsService interface in newsApi/Application/Services/INewsService.cs
- [x] T008 [P] Create CreateNewsDto and UpdateNewsDto classes in newsApi/Application/DTOs/ with proper validation attributes
- [x] T009 [P] Create FluentValidation validators in newsApi/Application/Validators/NewsValidator.cs
- [x] T010 Implement NewsService with repository pattern in newsApi/Application/Services/NewsService.cs

## Phase 3.4: Infrastructure Layer Implementation
- [x] T011 [P] Create MongoDbSettings configuration class in newsApi/Infrastructure/Data/Configurations/MongoDbSettings.cs
- [x] T012 [P] Create MongoDbContext with updated MongoDB driver patterns in newsApi/Infrastructure/Data/MongoDbContext.cs
- [x] T013 [P] Create JwtTokenService for JWT authentication in newsApi/Infrastructure/Security/JwtTokenService.cs
- [x] T014 [P] Create CacheService wrapper for memory caching in newsApi/Infrastructure/Caching/CacheService.cs
- [x] T015 Implement NewsRepository with MongoDB.Driver 2.28+ in newsApi/Infrastructure/Data/Repositories/NewsRepository.cs

## Phase 3.5: Presentation Layer Implementation
- [x] T016 [P] Create SecurityHeadersMiddleware for OWASP protection in newsApi/Presentation/Middleware/SecurityHeadersMiddleware.cs
- [x] T017 [P] Create ValidationMiddleware for FluentValidation integration in newsApi/Presentation/Middleware/ValidationMiddleware.cs
- [x] T018 [P] Create ServiceCollectionExtensions for dependency injection setup in newsApi/Presentation/Extensions/ServiceCollectionExtensions.cs
- [x] T019 Update NewsController to use new services and DTOs in newsApi/Presentation/Controllers/NewsController.cs

## Phase 3.6: Configuration and Security
- [x] T020 Remove sensitive data from appsettings.json and appsettings.Development.json
- [x] T021 Configure User Secrets for development environment sensitive data
- [x] T022 Update Program.cs to use .NET 8 minimal hosting model with dependency injection
- [x] T023 Configure JWT Bearer authentication in Program.cs
- [x] T024 Configure FluentValidation automatic validation in Program.cs
- [x] T025 Configure CORS policy to maintain existing origins in Program.cs

## Phase 3.7: Migration and Integration
- [x] T026 Create data migration script for existing MongoDB data to new schema
- [x] T027 Update Dockerfile to target .NET 8 runtime
- [x] T028 Configure health checks for MongoDB connectivity
- [x] T029 Verify all existing API endpoints maintain 100% compatibility

## Dependencies
- T001-T003 must complete before all other tasks (setup requirements)
- T004-T006 (Domain) before T007-T010 (Application) 
- T007-T010 (Application) before T011-T015 (Infrastructure)
- T004-T015 before T016-T019 (Presentation)
- T016-T019 before T020-T025 (Configuration)
- All implementation before T026-T029 (Migration)

## Parallel Execution Examples

### Phase 3.2 - Domain Layer (All Parallel):
```bash
# These tasks can run simultaneously as they create different files
Task T004: Create News entity in newsApi/Domain/Entities/News.cs
Task T005: Create INewsRepository interface in newsApi/Domain/Interfaces/INewsRepository.cs  
Task T006: Move CacheKeys to newsApi/Common/CacheKeys.cs
```

### Phase 3.3 - Application Layer (Mostly Parallel):
```bash
# Parallel group 1:
Task T007: Create INewsService interface in newsApi/Application/Services/INewsService.cs
Task T008: Create DTOs in newsApi/Application/DTOs/
Task T009: Create validators in newsApi/Application/Validators/NewsValidator.cs

# Sequential after group 1:
Task T010: Implement NewsService (depends on INewsService interface)
```

### Phase 3.4 - Infrastructure Layer (Mostly Parallel):
```bash
# Parallel group 1:
Task T011: Create MongoDbSettings in newsApi/Infrastructure/Data/Configurations/MongoDbSettings.cs
Task T012: Create MongoDbContext in newsApi/Infrastructure/Data/MongoDbContext.cs
Task T013: Create JwtTokenService in newsApi/Infrastructure/Security/JwtTokenService.cs
Task T014: Create CacheService in newsApi/Infrastructure/Caching/CacheService.cs

# Sequential after group 1:
Task T015: Implement NewsRepository (depends on MongoDbContext)
```

## Notes
- All tasks maintain 100% API endpoint compatibility as required
- No additional testing tasks per user specification
- JWT authentication implemented for protected endpoints (POST, PUT, DELETE)
- Public endpoints (GET) remain accessible without authentication
- FluentValidation provides comprehensive server-side input validation
- Security middleware protects against OWASP Top 10 vulnerabilities
- Clean Architecture principles enforced through folder organization and dependency rules
- MongoDB driver modernized to version 2.28+ with performance improvements