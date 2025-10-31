# News Portal - GitHub Copilot Instructions

> Professional instructions for GitHub Copilot to effectively assist with the News Portal project.

## ⚠️ Critical Guidelines

**Do not create unnecessary documentation.**
- Focus on code and completing tasks
- Only create files when explicitly requested
- Update existing documentation rather than creating new files
- No summary documents, completion reports, or explainer files

## 🎯 Quick Reference

### Architecture
- **Backend**: .NET 9, Clean Architecture, MongoDB, MinIO, JWT
- **Frontend**: Next.js 16, TypeScript, TailwindCSS, React Query
- **Development**: Docker Compose (all services containerized)
- **Production**: Azure Container Apps (auto-scaling 0-10 replicas)
- **Tests**: 178+ passing tests (xUnit, Moq)

### Key Points
- **NEVER** use `dotnet run` for development - use Docker Compose
- **Turkish support**: SlugHelper converts Turkish characters in URLs
- **Four main controllers**: NewsArticle, Auth, Seed, SocialMedia
- **Image storage**: MinIO (S3-compatible) with auto-bucket creation
- **Caching**: IMemoryCache (30-60 min TTL)

### Project Documentation
- **[BUILD.md](../docs/BUILD.md)** - Build instructions (Docker & local)
- **[RUN.md](../docs/RUN.md)** - Running & development guide
- **[DEPLOY.md](../docs/DEPLOY.md)** - Production deployment
- **[ARCHITECTURE.md](../docs/ARCHITECTURE.md)** - System architecture

## 🛠️ Tech Stack

### Backend Framework
- **.NET 10.0** - Latest version with C# 13+ features
- **ASP.NET Core** - Web API framework with minimal APIs support
- **Clean Architecture** - Domain, Application, Infrastructure, Presentation layers

### Database & Caching
- **MongoDB 2.30+** - Primary data store with MongoDB.Driver
- **IMemoryCache** - Built-in ASP.NET Core memory caching
- **Separate databases** - Dev, staging, and production environments

### Object Storage
- **MinIO** - S3-compatible object storage for images
- **ImageSharp** - Image processing and thumbnail generation
- **Bucket**: `news-images` with category-based folder structure

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

## 🐳 Docker Development Environment

**⚠️ IMPORTANT: This project uses Docker Compose for ALL backend services. DO NOT attempt to run backend services locally with `dotnet run`.**

### Container Architecture

The entire backend infrastructure runs in Docker containers orchestrated by `docker-compose.yml`:

1. **newsportal-backend** (.NET 9 API)
   - Port: `5000:8080` (host:container)
   - URL: `http://localhost:5000`
   - Health Check: `http://localhost:5000/health`
   - Auto-restarts on failure
   - Depends on MongoDB and MinIO

2. **newsportal-mongodb** (MongoDB 7.0)
   - Port: `27017:27017`
3. **newsportal-minio** (S3-compatible Object Storage)
   - API Port: `9000:9000`
   - Console UI: `http://localhost:9001`
   - Credentials: minioadmin/minioadmin123 (default)
   - Bucket: `news-images` (auto-created with public read access)
4. **minio-init** (MinIO Setup Container)
   - One-time initialization container
   - Creates `news-images` bucket automatically
   - Sets public download policy
   - Exits after successful setup

5. **newsportal-mongo-express** (MongoDB Admin UI)
   - Port: `8081:8081`
   - URL: `http://localhost:8081`
   - Basic Auth: admin/admin123 (default)
   - Credentials: minioadmin/minioadmin123 (default)
   - Bucket: `news-images` (auto-created)
   - Persistent volume: `newsportal_minio_data`

4. **newsportal-mongo-express** (MongoDB Admin UI)
   - Port: `8081:8081`
   - URL: `http://localhost:8081`
   - Basic Auth: admin/admin123 (default)

### Docker Commands

**Starting All Services:**
```bash
# Start all containers in detached mode
docker-compose up -d

# View logs for all services
docker-compose logs -f

# View logs for specific service
docker-compose logs -f newsapi
```

**Stopping Services:**
```bash
# Stop all containers (preserves data)
docker-compose down

# Stop and remove volumes (DELETES ALL DATA)
docker-compose down -v
```

**Rebuilding After Code Changes:**
```bash
# Rebuild and restart only the backend API
docker-compose up -d --build newsapi

# Rebuild all services
docker-compose up -d --build
```

**Checking Service Status:**
```bash
# List running containers
docker-compose ps

# Check backend health
curl http://localhost:5000/health

# View backend logs
docker-compose logs --tail=50 newsapi
```

**Accessing Container Shells:**
```bash
# Backend container bash
docker exec -it newsportal-backend bash

# MongoDB shell
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin

# MinIO client
docker exec -it newsportal-minio mc ls myminio/news-images
```

### Service URLs

| Service | URL | Purpose |
|---------|-----|---------|
| **Backend API** | http://localhost:5000 | REST API endpoints |
| **API Swagger** | http://localhost:5000/swagger | API documentation |
| **MongoDB** | mongodb://localhost:27017 | Database connection |
| **Mongo Express** | http://localhost:8081 | Database admin UI |
| **MinIO Console** | http://localhost:9001 | Object storage admin |
| **MinIO API** | http://localhost:9000 | S3-compatible API |

### Environment Variables

Configure via `.env` file in project root (create if missing):

```env
# MongoDB
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=password123
MONGO_DATABASE=NewsDb

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin123

# JWT
JWT_SECRET_KEY=your-super-secret-jwt-key-that-is-at-least-32-characters-long

# Mongo Express
MONGOEXPRESS_USER=admin
MONGOEXPRESS_PASSWORD=admin123

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
```

### Development Workflow

**DO THIS:**
✅ Make code changes in `backend/` directory
✅ Run `docker-compose up -d --build newsapi` to apply changes
✅ Test API via `http://localhost:5000`
✅ Check logs with `docker-compose logs -f newsapi`
✅ Use Swagger UI for API testing
✅ Access MongoDB via Mongo Express UI

**DON'T DO THIS:**
❌ `dotnet run` (backend is in Docker, will cause port conflicts)
❌ Install MongoDB locally (already in Docker)
❌ Install MinIO locally (already in Docker)
❌ Manual database setup (auto-configured in Docker)

### Common Docker Issues

**Problem: Port 5000 already in use**
```bash
# Check what's using the port
netstat -ano | findstr :5000

# Stop the conflicting container
docker-compose down
```

**Problem: Backend not responding**
```bash
# Check if container is running
docker-compose ps

# Restart backend
docker-compose restart newsapi

# View logs for errors
docker-compose logs --tail=100 newsapi
```

**Problem: Database connection failed**
```bash
# Ensure MongoDB is healthy
docker-compose ps mongodb

# Check MongoDB logs
docker-compose logs mongodb

# Restart all services
docker-compose restart
```

**Problem: Need fresh database**
```bash
# WARNING: This deletes all data
docker-compose down -v
docker-compose up -d
```

### Backend Development in Docker

When making changes to the backend code:

1. **Edit files** in `backend/` directory
2. **Rebuild container**: `docker-compose up -d --build newsapi`
## 📁 Project Structure

```
newsportal/
├── backend/                          # Main API project
│   ├── Domain/                       # Core business entities (no external dependencies)
│   │   ├── Entities/                # News entity models, ImageMetadata
│   │   └── Interfaces/              # Repository contracts (INewsRepository, IImageStorageService)
│   ├── Application/                 # Business logic & use cases
│   │   ├── DTOs/                    # Data Transfer Objects (CreateNewsDto, UpdateNewsDto)
│   │   ├── Services/                # Business services (NewsService, INewsService)
│   │   └── Validators/              # FluentValidation rules (NewsValidator)
│   ├── Infrastructure/              # External dependencies & implementations
│   │   ├── Data/                    # MongoDB context, repositories, models, mappers
│   │   ├── Caching/                 # Memory cache service
│   │   ├── Security/                # JWT token service
│   │   ├── Services/                # MinIO image storage, external service integrations
│   │   ├── HealthChecks/            # MongoDB health check
│   │   └── BackgroundJobs/          # SocialMediaFetcherService
│   ├── Presentation/                # API layer & HTTP concerns
│   │   ├── Controllers/             # API endpoints (NewsArticleController, AuthController, SeedController, SocialMediaController)
│   │   ├── Middleware/              # Security & validation middleware
│   │   └── Extensions/              # Service collection extensions
│   ├── Common/                      # Shared utilities (CacheKeys, SlugHelper, Mappers)
│   ├── Properties/                  # Launch settings and publish profiles
│   ├── ADMIN_IMAGE_UPLOAD_STRATEGY.md # Image upload architecture documentation
│   └── Program.cs                   # Application entry pointns
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
├── frontend/                        # Next.js 16 frontend
├── docker/                          # Docker configurations
├── scripts/                         # Development scripts
│   └── database/                    # Data migration scripts
**Important Folders**:
- `Domain/` - Pure business logic, no framework dependencies (Entities, Interfaces)
- `Application/` - Use cases and business rules (Services, DTOs, Validators)
- `Infrastructure/` - Data access, caching, external services (MongoDB, MinIO, JWT)
- `Presentation/` - Controllers, middleware, API contracts
- `Common/` - Shared utilities (SlugHelper for Turkish character conversion, CacheKeys, Mappers)
- `tests/` - Comprehensive test coverage
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
### Database & Data Access
- **Repository Pattern** - All data access through INewsRepository
- **No EF Core** - MongoDB with official driver only
- **Async queries** - All database operations asynchronous
- **Connection strings** - Never hardcode, use User Secrets or Key Vault
- **Projections** - Only query fields you need
- **Indexes** - Ensure proper MongoDB indexes for performance

### Image Storage & Processing
- **MinIO Integration** - All image operations through IImageStorageService
- **Thumbnail Generation** - Automatic thumbnail creation with ImageSharp
- **Image Validation** - File size (5MB max), format (jpg, png, webp, gif), dimensions
- **Folder Structure** - Organized by category: `news-images/{category}/{filename}`
- **Public Access** - Read-only public access, write requires authentication
- **URL Generation** - GetImageUrl() and GetThumbnailUrl() helper methods
- **Metadata Tracking** - ImageMetadata entity stores file info, dimensions, upload dateed
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

### Docker Commands (Primary Development Method)

**⚠️ ALWAYS use Docker for backend development. Backend runs in containers, NOT locally.**

- **Start All Services**
  ```bash
  docker-compose up -d              # Start in background
  docker-compose logs -f newsapi    # Follow backend logs
  ```

- **Rebuild After Code Changes**
  ```bash
  docker-compose up -d --build newsapi   # Rebuild only backend
  docker-compose up -d --build           # Rebuild all services
  ```

- **Stop Services**
  ```bash
  docker-compose down               # Stop containers
  docker-compose down -v            # Stop and delete data (WARNING)
  ```

- **View Logs**
  ```bash
  docker-compose logs -f newsapi    # Backend logs
  docker-compose logs -f mongodb    # Database logs
  docker-compose logs -f minio      # Storage logs
  ```

- **Service Status**
  ```bash
  docker-compose ps                 # List all containers
  curl http://localhost:5000/health # Check backend health
  ```

### Testing (Local Only)

**Tests run OUTSIDE Docker using local .NET SDK:**

```bash
dotnet test                                           # Run all tests
dotnet test --filter "FullyQualifiedName~Unit"        # Unit tests only
dotnet test --filter "FullyQualifiedName~Integration" # Integration tests only
```

**Note:** Integration tests may require Docker services (MongoDB) to be running.

### Database Management

- **Access MongoDB**: Open Mongo Express at `http://localhost:8081`
  - Username: `admin`
  - Password: `admin123`
- **MongoDB Shell**: `docker exec -it newsportal-mongodb mongosh -u admin -p password123`
- **Data Migration**: See `backend/Migration/data-migration.md`

### Configuration Management

- **Docker Environment**: `.env` file in project root (for container config)
- **Development Secrets**: `dotnet user-secrets set "Key" "Value"` (for local testing only)
- **Testing**: `appsettings.Testing.json` with test-specific values
- **Production**: Environment variables in `docker-compose.yml` or Azure Key Vault

### Health Checks

- **Backend Health**: `http://localhost:5000/health`
- **MinIO Health**: `http://localhost:9000/minio/health/live`
- **MongoDB Health**: Integrated into backend health check
- **All containers**: Health checks configured in `docker-compose.yml`

### Admin UI Access

- **MongoDB Admin**: http://localhost:8081 (Mongo Express)
- **MinIO Console**: http://localhost:9001 (Object Storage)
- **API Swagger**: http://localhost:5000/swagger (API Docs)

## 🧩 Common Development Tasks

### Adding a New API Endpoint
1. Define DTO in `Application/DTOs/`
2. Add validation in `Application/Validators/`
3. Implement service method in `Application/Services/NewsArticleService.cs`
4. Add controller action in `Presentation/Controllers/NewsArticleController.cs`
5. Add XML documentation comments
6. Write unit tests in `tests/Unit/`
7. Write integration tests in `tests/Integration/`

### Adding a New Entity
1. Create entity in `Domain/Entities/`
2. Define repository interface in `Domain/Interfaces/`
3. Create MongoDB model in `Infrastructure/Data/Models/`
4. Create mapper in `Infrastructure/Data/Mappers/` or `Common/Mappers/`
5. Implement repository in `Infrastructure/Data/Repositories/`
6. Register in DI container in `Presentation/Extensions/ServiceCollectionExtensions.cs`

### Adding Turkish Text Support
1. **Use SlugHelper** - For URL-friendly slugs from Turkish text
2. **Example**: `SlugHelper.GenerateSlug("Yapay Zeka Çağında")` → `"yapay-zeka-caginda"`
3. **Location**: `Common/SlugHelper.cs`
4. **Features**: Converts Turkish characters (ı→i, ğ→g, ü→u, ş→s, ö→o, ç→c), removes special chars, max 100 chars

### Testing Protected Endpoints
1. **Login**: POST to `/api/Auth/login` with `{ "username": "admin", "password": "admin123" }`
2. **Get Token**: Copy JWT token from response
3. **Swagger Auth**: Click "Authorize" button in Swagger UI, enter `Bearer <your-token>`
4. **Curl/Postman**: Add header `Authorization: Bearer <your-token>`

### MinIO Image Storage Settings
- **Endpoint**: `minio:9000` (Docker) or `localhost:9000` (external)
- **Bucket**: `news-images` (auto-created with public read access)
- **Max File Size**: 5MB per image
- **Allowed Formats**: jpg, jpeg, png, webp, gif
- **Thumbnail Size**: 400x300px (auto-generated)
- **Public URLs**: `http://localhost:9000/news-images/{objectKey}`

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

**Backend runs in Docker - debugging workflow is different:**

- **View Logs**: `docker-compose logs -f newsapi` (NOT dotnet run)
- **API Testing**: Use Swagger at `http://localhost:5000/swagger`
- **Database Inspection**: Open Mongo Express at `http://localhost:8081`
- **Health Check**: `curl http://localhost:5000/health`
- **Restart Service**: `docker-compose restart newsapi`
- **Full Rebuild**: `docker-compose up -d --build newsapi`
- **Container Shell**: `docker exec -it newsportal-backend bash`

**For local debugging with IDE:**
1. Stop Docker backend: `docker-compose stop newsapi`
2. Run locally: `dotnet run --project backend/newsApi.csproj`
3. Set breakpoints in Visual Studio/VS Code/Rider
4. Remember to restart Docker when done: `docker-compose start newsapi`

**Note:** Local run requires MongoDB to be accessible. Keep Docker MongoDB running.

## 🚫 Patterns to Avoid

### Docker & Deployment
- **Don't run `dotnet run` for backend** - Backend is in Docker, use `docker-compose up -d`
- **Don't install MongoDB locally** - MongoDB is in Docker container
- **Don't install MinIO locally** - MinIO is in Docker container
- **Don't hardcode localhost URLs** - Use service names in Docker (mongodb, minio)
- **Don't forget to rebuild after code changes** - `docker-compose up -d --build newsportal-backend`
- **Don't skip Docker health checks** - All services have health checks configured
- **Don't ignore container dependencies** - Backend depends on MongoDB and MinIO being healthy

### Code Quality
- **Don't use static state** - Except for constants
- **Don't use service locator** - Always use constructor injection
- **Don't put logic in controllers** - Keep controllers thin, delegate to services
- **Don't expose domain entities** - Use DTOs for API responses
- **Don't hardcode configuration** - Use appsettings.json and environment variables
- **Don't use EF Core** - This is a MongoDB project
- **Don't ignore validation** - Always validate user input
- **Don't catch generic exceptions** - Be specific with exception handling
- **Don't use async void** - Use async Task instead
- **Don't block on async code** - Avoid .Result or .Wait()

## 📚 Additional References

### Official Documentation
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/current/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [xUnit](https://xunit.net/)

### Project Documentation
- `NEWS_API_DOCUMENTATION.md` - Complete API reference
- `SWAGGER_TESTING_GUIDE.md` - Interactive testing guide
- `AZURE_DEPLOYMENT.md` - Azure Container Apps deployment guide
- `backend/ADMIN_IMAGE_UPLOAD_STRATEGY.md` - Image upload architecture and future plans
- `specs/002-modernize-net-core/spec.md` - Architecture specifications
- `scripts/database/data-migration.md` - Data migration guide
- `tests/TEST_COVERAGE_REPORT.md` - Test coverage details

### Architecture Resources
- [Clean Architecture by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [Microsoft Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/)

## 🎯 Current API Controllers

### NewsArticleController (`/api/NewsArticle`)
**Purpose**: Main CRUD operations for news articles

**Public Endpoints** (No Auth):
- `GET /api/NewsArticle` - Get all news with optional category/type filters
- `GET /api/NewsArticle/{id}` - Get specific news by ID
- `GET /api/NewsArticle/slug/{slug}` - Get news by slug (Turkish-friendly URLs)
- `GET /api/NewsArticle/category/{category}` - Get news by category
- `GET /api/NewsArticle/trending` - Get trending news

**Protected Endpoints** (Requires JWT):
- `POST /api/NewsArticle` - Create new article (Admin only)
- `PUT /api/NewsArticle/{id}` - Update article (Admin only)
- `DELETE /api/NewsArticle/{id}` - Delete article (Admin only)
- `POST /api/NewsArticle/{id}/upload-image` - Upload article image (Admin only)

**Key Features**:
- Response caching for all GET endpoints (30 min)
- Turkish slug support via SlugHelper
- MinIO image integration for uploads
- Automatic thumbnail generation

### AuthController (`/api/Auth`)
**Purpose**: JWT-based authentication

**Endpoints**:
- `POST /api/Auth/login` - Login and get JWT token
  - Default credentials: `admin` / `admin123`
  - Returns: JWT token with 24h expiration

### SeedController (`/api/Seed`)
**Purpose**: Database initialization with sample data

**Endpoints**:
- `POST /api/Seed/seed-database` - Populate DB with 50+ news articles
- Categories: Technology, Sports, World, Business, Science, Health, Entertainment
- Mix of Unsplash images and MinIO-hosted images

### SocialMediaController (`/api/SocialMedia`)
**Purpose**: Social media integrations

**Endpoints**:
- `GET /api/SocialMedia/twitter` - Get Twitter posts
- Background service fetches social media content automatically

## 🚀 Azure Deployment

### Current Infrastructure
- **Resource Group**: `newsportal-rg` (East US)
- **Container Registry**: `newsportal.azurecr.io` (ACR Basic)
- **Backend**: Azure Container Apps (newsportal-backend)
- **Database**: MongoDB Atlas (M0 Free Tier)
- **Storage**: Cloudflare R2 (S3-compatible, 10GB free)
- **Frontend**: Azure Static Web Apps (Free tier)

### Deployment Scripts
- `deploy-to-azure.ps1` - Deploy backend to Container Apps
- `docker-rebuild.ps1` - Rebuild and restart local containers
- `docker-start.ps1` - Start all Docker services
- `docker-stop.ps1` - Stop all Docker services
- `docker-status.ps1` - Check service status
- `docker-logs.ps1` - View container logs

### Environment Configuration
**Development** (Docker Compose):
- MongoDB: `mongodb://admin:password123@mongodb:27017/NewsDb`
- MinIO: `minio:9000` (internal), `localhost:9000` (external)
- JWT: Development secret key

**Production** (Azure):
- MongoDB: MongoDB Atlas connection string
- Storage: Cloudflare R2 endpoint
- JWT: Secure secret via Container App secrets
- HTTPS: Automatic SSL via Azure

### Key Azure Features
- **Health Checks**: `/health` endpoint with MongoDB ping
- **Auto-scaling**: Scale to 0 when idle (cost savings)
- **Environment Variables**: Managed via Container App configuration
- **Logging**: Azure Monitor integration
- **Secrets**: Stored securely in Container App secrets

## 💡 Tips for Effective Copilot Usage

1. **Provide context** - Include entity names, layer names, and purpose in prompts
2. **Be specific** - "Add a validator for CreateNewsDto with max length 500 for Caption"
3. **Reference existing patterns** - "Following the pattern in NewsArticleService, add a new method..."
4. **Specify layer** - "In the Application layer, create..." or "Add to NewsArticleController..."
5. **Include validation** - Always ask for validation rules when creating DTOs
6. **Request tests** - "...and write unit tests for the new method"
7. **Mention Turkish support** - "Generate slug using SlugHelper for Turkish characters"
8. **Iterate** - Review generated code and refine with follow-up prompts

## 🔄 Iteration & Review

- **Review all generated code** - Copilot suggestions should be reviewed before committing
- **Run tests after changes** - Ensure `dotnet test` passes
- **Check code style** - Use Visual Studio or Rider code inspections
- **Update documentation** - Keep XML comments and README in sync
- **Follow PR process** - Create feature branches, write meaningful commit messages

---

**Last Updated**: October 2025  
**Copilot Version Compatibility**: GitHub Copilot with VS Code or Visual Studio 2022+
