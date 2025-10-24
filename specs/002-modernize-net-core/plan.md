# Implementation Plan: .NET 8 Modernization with Clean Architecture

**Branch**: `002-modernize-net-core` | **Date**: 2025-09-27 | **Spec**: [link](spec.md)  
**Input**: Feature specification from `/specs/002-modernize-net-core/spec.md`

## Summary

Modernize existing .NET Core 3.1 News API to .NET 8 LTS with Clean Architecture principles, implementing JWT authentication, FluentValidation, secure configuration management, and maintaining all existing API functionality while following constitutional requirements.

## Technical Context

**Language/Version**: .NET 8 LTS with C# 12  
**Primary Dependencies**: ASP.NET Core 8.0, MongoDB.Driver 2.28+, FluentValidation 11.3+, JWT Bearer Authentication  
**Storage**: MongoDB with clean schema design and migration scripts  
**Testing**: No additional testing beyond existing functionality  
**Target Platform**: Cross-platform (.NET 8 runtime), Docker containers  
**Project Type**: Single project with Clean Architecture folders  
**Performance Goals**: Sub-200ms response times for cached content, maintain existing performance  
**Constraints**: 100% API endpoint compatibility, preserve existing CORS origins  
**Scale/Scope**: Existing News API functionality with modern architecture

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. API-First Design
✅ **PASS** - All existing endpoints preserved with RESTful conventions, CORS maintained, JSON responses standard

### II. Cross-Platform Compatibility  
✅ **PASS** - Client-agnostic design preserved, no client-specific logic added

### III. CRUD Operations Standard
✅ **PASS** - Complete CRUD operations maintained through repository pattern with INewsService interface

### IV. Performance & Caching
✅ **PASS** - Existing caching functionality preserved with memory cache implementation

### V. Repository Pattern & Dependency Injection
✅ **PASS** - Repository pattern with clear interfaces, dependency injection for all services

## Project Structure

### Documentation (this feature)
```
specs/002-modernize-net-core/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
newsApi/
├── Domain/
│   ├── Entities/
│   │   └── News.cs
│   ├── Interfaces/
│   │   └── INewsRepository.cs
│   └── ValueObjects/
├── Application/
│   ├── Services/
│   │   ├── INewsService.cs
│   │   └── NewsService.cs
│   ├── DTOs/
│   ├── Validators/
│   │   └── NewsValidator.cs
│   └── Mappings/
├── Infrastructure/
│   ├── Data/
│   │   ├── MongoDbContext.cs
│   │   ├── Repositories/
│   │   │   └── NewsRepository.cs
│   │   └── Configurations/
│   │       └── MongoDbSettings.cs
│   ├── Security/
│   │   └── JwtTokenService.cs
│   └── Caching/
│       └── CacheService.cs
├── Presentation/
│   ├── Controllers/
│   │   └── NewsController.cs
│   ├── Middleware/
│   │   ├── SecurityHeadersMiddleware.cs
│   │   └── ValidationMiddleware.cs
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs
├── Common/
│   └── CacheKeys.cs
├── Program.cs
├── appsettings.json (cleaned of sensitive data)
└── newsApi.csproj (updated to .NET 8)
```

**Structure Decision**: Single project with Clean Architecture folders organized within the existing newsApi project, maintaining clear separation of concerns and dependency rules.

## Phase 0: Outline & Research

1. **Extract unknowns from Technical Context**:
   - Research JWT Bearer authentication implementation patterns for .NET 8
   - Investigate FluentValidation integration with minimal API and controllers
   - Study .NET 8 minimal hosting model migration from Startup.cs pattern
   - Research MongoDB driver 2.28+ breaking changes and migration requirements
   - Analyze clean architecture folder organization best practices for single projects

2. **Generate and dispatch research agents**:
   ```
   Research JWT Bearer authentication setup for .NET 8 API
   Research FluentValidation integration patterns with ASP.NET Core 8
   Research .NET 8 minimal hosting model migration strategies
   Research MongoDB driver 2.28+ migration requirements and breaking changes
   Research Clean Architecture folder organization within single project
   ```

3. **Consolidate findings** in `research.md`:
   - Decision: JWT Bearer with Microsoft.AspNetCore.Authentication.JwtBearer 8.0+
   - Decision: FluentValidation.AspNetCore 11.3+ with automatic validation
   - Decision: Program.cs minimal hosting with WebApplicationBuilder
   - Decision: MongoDB.Driver 2.28+ with updated connection patterns
   - Decision: Clean Architecture folders with namespace organization

**Output**: research.md with all technology decisions and migration approaches

## Phase 1: Design & Contracts

*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - News entity with clean domain design
   - NewsRepository interface for data access abstraction
   - Configuration entities for MongoDB and JWT settings
   - Validation rules and business logic constraints

2. **Generate API contracts** from functional requirements:
   - Preserve existing REST endpoints: GET, POST, PUT, DELETE /api/news
   - Add JWT authentication headers to protected endpoints
   - Include validation error response formats
   - Maintain existing CORS policy endpoints
   - Output OpenAPI schema to `/contracts/`

3. **Extract user scenarios** from clarified requirements:
   - News CRUD operations with authentication
   - Secure configuration retrieval scenarios
   - Input validation and error handling flows
   - MongoDB connection and caching scenarios

4. **Update agent file incrementally**:
   - Execute: `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot`
   - Add .NET 8, FluentValidation, JWT authentication context
   - Update project structure information
   - Preserve existing manual additions

**Output**: data-model.md, /contracts/*, quickstart.md, .github/copilot-instructions.md

## Phase 2: Task Planning Approach

*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load implementation artifacts from Phase 1 design docs
- Generate setup tasks for .NET 8 upgrade and package updates
- Create Clean Architecture restructuring tasks for folder organization
- Generate security implementation tasks for JWT and validation
- Create configuration migration tasks for User Secrets and Azure Key Vault
- Generate MongoDB modernization tasks with updated drivers

**Ordering Strategy**:
- Setup and upgrade tasks first (.NET 8, packages, project structure)
- Core architecture implementation (Clean Architecture folders, interfaces)
- Security implementation (JWT, validation, secure configuration)
- Data layer modernization (MongoDB, repository pattern)
- Integration and configuration (dependency injection, middleware)
- Documentation and migration scripts

**Estimated Output**: 15-20 numbered, ordered tasks focusing on modernization without additional testing

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation

*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run existing functionality verification, execute quickstart.md)

## Complexity Tracking

No constitutional violations identified. All requirements align with existing principles:
- API-First Design maintained through endpoint preservation
- Repository Pattern enhanced with Clean Architecture
- Performance & Caching preserved with modern implementations
- Cross-Platform Compatibility maintained through JSON APIs

## Progress Tracking

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All technical decisions clarified
- [x] Clean Architecture approach defined

---
*Based on Constitution v1.0.0 - See `/memory/constitution.md`*
