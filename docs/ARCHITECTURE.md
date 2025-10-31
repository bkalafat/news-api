# Architecture Documentation

## Overview

News Portal is a modern full-stack application built with Clean Architecture principles, featuring a .NET 9 backend API and Next.js 16 frontend.

## Technology Stack

### Backend

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Framework** | ASP.NET Core | 9.0 | Web API framework |
| **Language** | C# | 13+ | Primary programming language |
| **Database** | MongoDB | 7.0+ | NoSQL document database |
| **Object Storage** | MinIO | Latest | S3-compatible image storage |
| **Caching** | IMemoryCache | Built-in | In-memory application cache |
| **Authentication** | JWT Bearer | Built-in | Token-based auth |
| **Validation** | FluentValidation | 11+ | Request validation |
| **Testing** | xUnit + Moq | Latest | Unit & integration tests |
| **Documentation** | Swashbuckle | Latest | OpenAPI/Swagger docs |

### Frontend

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Framework** | Next.js | 16 | React framework with SSR |
| **Language** | TypeScript | 5+ | Type-safe development |
| **Styling** | TailwindCSS | 4 | Utility-first CSS |
| **UI Components** | Shadcn/ui | Latest | Accessible components |
| **Data Fetching** | React Query | 5+ | Server state management |
| **Internationalization** | next-intl | Latest | Turkish support |
| **Testing** | Vitest + Playwright | Latest | Unit & E2E tests |

## Architecture Patterns

### Clean Architecture (Backend)

```
┌─────────────────────────────────────────────┐
│           Presentation Layer                 │
│  - Controllers (HTTP endpoints)             │
│  - Middleware (validation, auth, errors)    │
│  - Extensions (service registration)        │
└────────────────┬────────────────────────────┘
                 │ depends on ↓
┌────────────────┴────────────────────────────┐
│           Application Layer                  │
│  - Services (business logic)                │
│  - DTOs (data transfer objects)             │
│  - Validators (FluentValidation rules)      │
└────────────────┬────────────────────────────┘
                 │ depends on ↓
┌────────────────┴────────────────────────────┐
│              Domain Layer                    │
│  - Entities (business models)               │
│  - Interfaces (repository contracts)        │
│  - No external dependencies                 │
└────────────────┬────────────────────────────┘
                 ↑ implements
┌────────────────┴────────────────────────────┐
│          Infrastructure Layer                │
│  - Data (MongoDB repos, mappers)           │
│  - Services (MinIO, Reddit, etc.)          │
│  - Security (JWT token service)             │
│  - Caching (memory cache implementation)    │
└─────────────────────────────────────────────┘
```

**Key Principles:**
- Dependencies point inward (Presentation → Application → Domain)
- Domain layer has no external dependencies
- Infrastructure implements domain interfaces
- Business logic isolated in Application/Domain layers

### Project Structure

```
backend/
├── Domain/                    # Core business entities
│   ├── Entities/             # News, ImageMetadata, etc.
│   └── Interfaces/           # INewsRepository, IImageStorageService
│
├── Application/              # Business logic & use cases
│   ├── DTOs/                # CreateNewsDto, UpdateNewsDto, etc.
│   ├── Services/            # NewsService, SocialMediaPostService
│   └── Validators/          # FluentValidation rules
│
├── Infrastructure/           # External dependencies
│   ├── Data/                # MongoDB context, repositories
│   ├── Services/            # MinIO, Reddit, etc.
│   ├── Security/            # JWT token service
│   ├── Caching/             # Memory cache wrapper
│   └── BackgroundJobs/      # Social media fetcher
│
├── Presentation/            # API layer
│   ├── Controllers/         # NewsArticleController, AuthController
│   ├── Middleware/          # ValidationMiddleware
│   └── Extensions/          # ServiceCollectionExtensions
│
└── Common/                  # Shared utilities
    └── Helpers/             # SlugHelper (Turkish character support)
```

### Frontend Architecture (Next.js)

```
frontend/
├── app/                     # App Router (Next.js 16)
│   ├── [locale]/           # Internationalized routes
│   ├── layout.tsx          # Root layout
│   └── page.tsx            # Homepage
│
├── components/             # React components
│   ├── ui/                # Shadcn/ui primitives
│   ├── news/              # News-specific components
│   └── layout/            # Header, Footer, etc.
│
├── lib/                   # Utilities
│   ├── api/              # API client (fetch wrappers)
│   ├── hooks/            # Custom React hooks
│   └── utils/            # Helper functions
│
├── messages/             # i18n translations (Turkish)
└── public/               # Static assets
```

## Data Flow

### API Request Flow

```
1. HTTP Request
   ↓
2. Middleware Pipeline
   - ValidationMiddleware (validates DTOs)
   - Authentication (JWT validation)
   - CORS
   ↓
3. Controller
   - Route to appropriate action
   - Bind request data to DTOs
   ↓
4. Service Layer
   - Business logic execution
   - Cache check (if applicable)
   ↓
5. Repository Layer
   - MongoDB query execution
   - Domain entity mapping
   ↓
6. Database (MongoDB)
   - Document retrieval/update
   ↓
7. Response Flow (reverse)
   - Entity → DTO mapping
   - Cache update (if applicable)
   - HTTP response serialization
```

### Image Upload Flow

```
1. Client uploads image via multipart/form-data
   ↓
2. Controller validates file (size, format)
   ↓
3. ImageStorageService (MinIO)
   - Resize image (max 1920x1080)
   - Generate thumbnail (300x200)
   - Upload to MinIO: news-images/{category}/
   ↓
4. Store metadata in MongoDB
   - Image URL, thumbnail URL
   - Dimensions, file size
   ↓
5. Return image URLs to client
```

### Authentication Flow

```
1. POST /api/auth/login
   - Username: admin
   - Password: admin123
   ↓
2. TokenService validates credentials
   ↓
3. Generate JWT token (60 min expiration)
   - Claims: username, role, expiration
   ↓
4. Return token to client
   ↓
5. Client includes token in subsequent requests
   - Header: Authorization: Bearer {token}
   ↓
6. JwtBearerMiddleware validates token
   ↓
7. Request proceeds if valid
```

## Database Schema (MongoDB)

### News Collection

```javascript
{
  "_id": ObjectId("..."),
  "Id": "guid-string",
  "Category": "technology",
  "Type": "article",
  "Caption": "News headline",
  "Url": "url-friendly-slug",
  "Keywords": "keyword1, keyword2",
  "SocialTags": "#tech #news",
  "Summary": "Brief summary",
  "ImgPath": "http://minio:9000/news-images/technology/image.jpg",
  "ImgAlt": "Image description",
  "ImgCredit": "Photo source",
  "Content": "Full article content",
  "ExpressDate": ISODate("2025-10-30T00:00:00Z"),
  "CreateDate": ISODate("2025-10-30T21:00:00Z"),
  "Priority": 50,
  "IsActive": true,
  "ShowComment": true,
  "ImageMetadata": {
    "FileName": "image.jpg",
    "FileSize": 245678,
    "ContentType": "image/jpeg",
    "UploadedDate": ISODate("2025-10-30T21:00:00Z"),
    "Width": 1920,
    "Height": 1080,
    "ThumbnailUrl": "http://minio:9000/news-images/technology/thumb_image.jpg",
    "ThumbnailWidth": 300,
    "ThumbnailHeight": 200
  }
}
```

**Indexes:**
- `{ "Id": 1 }` - Primary lookup
- `{ "Url": 1 }` - Slug-based queries
- `{ "Category": 1, "ExpressDate": -1 }` - Category browsing
- `{ "IsActive": 1, "ExpressDate": -1 }` - Active news sorting

### SocialMediaPosts Collection

```javascript
{
  "_id": ObjectId("..."),
  "Id": "guid-string",
  "Platform": "reddit",
  "SourceId": "post-id",
  "SourceUrl": "https://reddit.com/...",
  "Title": "Post title",
  "Content": "Post content",
  "Author": "username",
  "Score": 1234,
  "CommentCount": 56,
  "CreatedAt": ISODate("..."),
  "Subreddit": "programming",
  "Tags": ["coding", "tech"],
  "IsProcessed": false
}
```

## Caching Strategy

### Memory Cache (IMemoryCache)

**Cached Data:**
- All news articles (key: `all_news`, TTL: 30 min)
- News by category (key: `news_category_{category}`, TTL: 30 min)
- News by ID (key: `news_{id}`, TTL: 60 min)

**Cache Invalidation:**
- On create: Clear `all_news` and category-specific caches
- On update: Clear specific news cache and `all_news`
- On delete: Clear specific news cache and `all_news`

```csharp
// Cache read
if (_cache.TryGetValue(cacheKey, out News cachedNews))
{
    return cachedNews;
}

// Cache write
_cache.Set(cacheKey, news, TimeSpan.FromMinutes(30));
```

## Security

### Authentication & Authorization

- **JWT Bearer Tokens**: Stateless authentication
- **Secret Key**: 32+ character secret (stored in Azure Key Vault in production)
- **Token Expiration**: 60 minutes (configurable)
- **Protected Endpoints**: POST, PUT, DELETE require authentication

### Input Validation

- **FluentValidation**: Server-side validation for all DTOs
- **Validation Middleware**: Automatically validates requests before reaching controllers
- **Sanitization**: SQL injection not applicable (NoSQL), XSS handled by framework

### CORS Policy

```csharp
// Development
.WithOrigins("http://localhost:3000", "http://localhost:5000")

// Production (configure in code)
.WithOrigins("https://your-frontend-domain.com")
```

### Security Headers

- **HSTS**: HTTP Strict Transport Security
- **X-Content-Type-Options**: nosniff
- **X-Frame-Options**: DENY
- **Content Security Policy**: Defined for API responses

## Performance Optimizations

### Backend

1. **Memory Caching**: Reduces database load for frequently accessed data
2. **Async/Await**: All I/O operations are asynchronous
3. **MongoDB Indexes**: Fast queries on Category, Url, ExpressDate
4. **Lazy Loading**: Related entities loaded on demand
5. **Image Optimization**: Images resized to max dimensions (1920x1080)

### Frontend

1. **Server-Side Rendering (SSR)**: Fast initial page load
2. **Static Generation**: Pre-rendered pages where possible
3. **Image Optimization**: Next.js automatic image optimization
4. **Code Splitting**: Automatic route-based splitting
5. **React Query Caching**: Reduces API calls

## Monitoring & Logging

### Application Insights (Production)

- Request/response tracking
- Exception logging
- Performance metrics
- Custom events

### Structured Logging

```csharp
_logger.LogInformation("News created: {Id}, Category: {Category}", 
    news.Id, news.Category);

_logger.LogError(ex, "Error creating news: {Error}", ex.Message);
```

## Scalability

### Horizontal Scaling

- **Backend**: Azure Container Apps auto-scales based on HTTP requests
- **Frontend**: Vercel/Azure Static Web Apps global CDN
- **Database**: MongoDB Atlas supports replica sets and sharding
- **Object Storage**: MinIO distributed mode or Cloudflare R2

### Current Limits

- **Max Request Size**: 5 MB (for image uploads)
- **Connection Pool**: 100 connections to MongoDB
- **Cache Size**: Limited by available memory (~500 MB max)
- **Auto-scale**: 0-10 replicas (configurable)

## Testing Strategy

### Unit Tests (178 tests)

- Service layer business logic
- Validators (FluentValidation rules)
- DTOs and mappers
- Helper utilities (SlugHelper)

### Integration Tests

- Controller endpoints (HTTP)
- Repository operations (MongoDB)
- Authentication flow

### Test Coverage

- **Target**: 80%+ code coverage
- **Critical paths**: 100% coverage (authentication, data validation)

## Deployment Environments

| Environment | URL | Auto-Deploy | Database | Storage |
|-------------|-----|-------------|----------|---------|
| **Local** | localhost:5000 | No | Local MongoDB | Local MinIO |
| **Production** | Azure Container Apps | Yes (GitHub Actions) | MongoDB Atlas | MinIO/R2 |

## Future Enhancements

### Planned Features

- [ ] Redis distributed cache (replace IMemoryCache)
- [ ] Elasticsearch for full-text search
- [ ] Azure Blob Storage support (alternative to MinIO)
- [ ] Rate limiting per API key
- [ ] GraphQL API endpoint
- [ ] Real-time updates (SignalR)
- [ ] Admin dashboard (separate app)

### Infrastructure Improvements

- [ ] Multi-region deployment
- [ ] Database replication for high availability
- [ ] CDN for image delivery
- [ ] Blue-green deployments
- [ ] Automated backup and restore

## Dependencies Management

### Backend NuGet Packages

- MongoDB.Driver (2.30+)
- FluentValidation.AspNetCore (11+)
- Microsoft.AspNetCore.Authentication.JwtBearer (9.0)
- Minio (6+)
- Swashbuckle.AspNetCore (7+)
- xUnit, Moq (testing)

### Frontend NPM Packages

- next (16+)
- react, react-dom (19+)
- tailwindcss (4+)
- @tanstack/react-query (5+)
- next-intl, framer-motion, etc.

## Contributing

For architecture changes:
1. Follow Clean Architecture principles
2. Maintain backward compatibility
3. Update documentation
4. Add tests for new features
5. Ensure no breaking changes without version bump

---

**Last Updated**: October 2025  
**Architecture Version**: 2.0
