# Research: .NET 8 Modernization Technologies

## JWT Bearer Authentication for .NET 8

**Decision**: Use Microsoft.AspNetCore.Authentication.JwtBearer 8.0.8  
**Rationale**: Official Microsoft package with full .NET 8 support, mature and well-documented  
**Alternatives considered**: 
- IdentityServer4 (deprecated, overengineered for this use case)
- Custom JWT implementation (security risk, maintenance overhead)

**Implementation Pattern**:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
```

## FluentValidation Integration

**Decision**: FluentValidation.AspNetCore 11.3.0  
**Rationale**: Rich validation API, excellent .NET 8 support, automatic model validation integration  
**Alternatives considered**:
- Data Annotations (limited validation rules)
- Manual validation (code duplication, maintenance overhead)

**Integration Pattern**:
```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<NewsValidator>();
```

## .NET 8 Minimal Hosting Model

**Decision**: Migrate from Startup.cs to Program.cs with WebApplicationBuilder  
**Rationale**: Modern .NET 8 pattern, reduced boilerplate, better performance  
**Alternatives considered**:
- Keep Startup.cs pattern (legacy approach, not recommended for new .NET 8 projects)

**Migration Pattern**:
```csharp
var builder = WebApplication.CreateBuilder(args);
// Configure services
var app = builder.Build();
// Configure pipeline
app.Run();
```

## MongoDB Driver Modernization

**Decision**: MongoDB.Driver 2.28.0  
**Rationale**: Latest stable version with .NET 8 support, performance improvements, security updates  
**Alternatives considered**:
- Keep MongoDB.Driver 2.11.0 (security vulnerabilities, performance issues)

**Breaking Changes**: 
- Updated connection string handling
- Modern async patterns
- Improved serialization options

## Clean Architecture in Single Project

**Decision**: Folder-based organization with namespace separation  
**Rationale**: Maintains existing project structure while implementing Clean Architecture principles  
**Alternatives considered**:
- Multiple projects (increased complexity, not requested)
- No architectural changes (doesn't meet modernization requirements)

**Folder Structure**:
- Domain/ (entities, interfaces, value objects)
- Application/ (services, DTOs, validators)
- Infrastructure/ (data access, external services)
- Presentation/ (controllers, middleware)

## Secure Configuration Management

**Decision**: .NET Secret Manager for development, Azure Key Vault for production  
**Rationale**: Standard Microsoft practices, built-in .NET 8 support  
**Alternatives considered**:
- Environment variables only (less secure for development)
- Configuration files (security risk)

**Implementation**: 
- Remove connection strings from appsettings.json
- Use `dotnet user-secrets` for development
- Implement IConfiguration with Azure Key Vault provider