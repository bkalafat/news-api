<!--
Sync Impact Report:
- Version change: Template → 1.0.0 (Initial constitution creation for .NET Core News API)
- Added principles: API-First Design, Cross-Platform Compatibility, CRUD Operations Standard, Performance & Caching, Repository Pattern
- Added sections: Technology Stack Requirements, Development Standards
- Templates reviewed: ✅ plan-template.md (aligned), ✅ spec-template.md (compatible), ✅ tasks-template.md (aligned with TDD principle)
- Modified principles impact: Constitution Check section in plan-template will now validate API-first design, CRUD completeness, and repository pattern usage
- Follow-up TODOs: None - all templates are compatible with the news API constitution
-->

# News API Constitution

## Core Principles

### I. API-First Design
Every feature MUST expose functionality through RESTful HTTP endpoints. APIs MUST follow RESTful conventions: GET for retrieval, POST for creation, PUT for updates, DELETE for removal. Response format MUST be consistent JSON with proper HTTP status codes. All endpoints MUST support CORS for cross-origin access to serve multiple client types.

**Rationale**: This news API serves both Next.js web frontend and React Native mobile app, requiring consistent cross-platform API contracts.

### II. Cross-Platform Compatibility
The API MUST remain client-agnostic and support multiple frontend technologies simultaneously. All responses MUST use standard JSON format. CORS configuration MUST accommodate all legitimate client origins (web, mobile, development environments). No client-specific logic MUST exist in the API layer.

**Rationale**: Single backend serves web (Next.js) and mobile (React Native) clients, requiring platform-neutral design.

### III. CRUD Operations Standard
Every data entity MUST implement complete CRUD operations (Create, Read, Update, Delete) through standard HTTP methods. Services MUST implement INewsService interface pattern for consistency. All operations MUST include proper error handling and validation. Mock services MUST mirror production service contracts exactly.

**Rationale**: Standard CRUD ensures predictable API behavior and enables rapid development of new features.

### IV. Performance & Caching
All GET operations MUST implement caching strategies using memory cache or appropriate caching mechanism. Database queries MUST be optimized to minimize response times. View counts and analytics MUST be tracked efficiently. API responses MUST include appropriate caching headers.

**Rationale**: News content requires fast delivery for good user experience across web and mobile platforms.

### V. Repository Pattern & Dependency Injection
All data access MUST follow repository pattern with clear interfaces (e.g., INewsService). Services MUST be registered with dependency injection container. Implementation details MUST be abstracted behind interfaces to enable testing and mock implementations. Database configurations MUST be externalized through IConfiguration.

**Rationale**: Enables testability, maintainability, and flexibility to swap implementations (production vs mock services).

## Technology Stack Requirements

The API MUST use .NET Core 3.1+ with ASP.NET Core Web API framework. Database operations MUST use MongoDB with official MongoDB.Driver package. Configuration MUST use ASP.NET Core configuration system with appsettings.json. Memory caching MUST use Microsoft.Extensions.Caching.Memory. Docker containerization MUST be supported through Dockerfile.

CORS policy MUST be configured to allow specified origins for web and mobile clients. All models MUST use appropriate data annotations and MongoDB BSON attributes. Startup configuration MUST follow dependency injection best practices.

## Development Standards

All new endpoints MUST include proper HTTP method attributes and route templates. Controller actions MUST return appropriate HTTP status codes (200, 201, 400, 404, 500). Model validation MUST be implemented using data annotations or custom validators. Error handling MUST provide meaningful error messages without exposing sensitive information.

Database connection strings and sensitive configuration MUST use user secrets or environment variables. All service methods MUST implement proper exception handling. API documentation MUST be maintained through XML comments or Swagger integration.

## Governance

This constitution supersedes all other development practices and architectural decisions. All pull requests MUST verify compliance with these principles before merge approval. Any deviation from these principles MUST be justified in writing and approved by project maintainers.

Constitution amendments require documentation of the change rationale, impact assessment, and migration plan for existing code. Complexity that violates these principles MUST be simplified or justified with performance/business requirements.

**Version**: 1.0.0 | **Ratified**: 2025-09-27 | **Last Amended**: 2025-09-27