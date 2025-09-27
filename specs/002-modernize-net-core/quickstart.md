# Quickstart: .NET 8 News API Modernization

## Prerequisites

- .NET 8 SDK installed
- MongoDB running locally or connection string to remote instance
- JWT signing key configured in user secrets
- Visual Studio Code or Visual Studio 2022

## Setup Steps

### 1. Configure User Secrets (Development)

```bash
cd newsApi
dotnet user-secrets init
dotnet user-secrets set "MongoDbSettings:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDbSettings:DatabaseName" "NewsDb"
dotnet user-secrets set "MongoDbSettings:NewsCollectionName" "News"
dotnet user-secrets set "JwtSettings:SecretKey" "your-super-secret-jwt-key-here"
dotnet user-secrets set "JwtSettings:Issuer" "NewsApi"
dotnet user-secrets set "JwtSettings:Audience" "NewsApiClients"
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Core Scenarios

### Scenario 1: Public News Access

**Test GET all news (public endpoint)**:
```bash
curl -X GET "https://localhost:5001/api/news" \
  -H "Accept: application/json"
```

**Expected Result**: 200 OK with array of news articles

**Test GET specific news by ID (public endpoint)**:
```bash
curl -X GET "https://localhost:5001/api/news/{news-id}" \
  -H "Accept: application/json"
```

**Expected Result**: 200 OK with news article details

### Scenario 2: Authenticated News Management

**Test POST create news (requires JWT)**:
```bash
curl -X POST "https://localhost:5001/api/news" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Technology",
    "type": "news",
    "caption": "Test News Article",
    "summary": "This is a test news article summary",
    "content": "Full content of the test news article...",
    "subjects": ["tech", "testing"],
    "authors": ["Test Author"],
    "expressDate": "2025-09-27T10:00:00Z",
    "priority": 1,
    "isActive": true,
    "url": "test-news-article"
  }'
```

**Expected Result**: 201 Created with news article details

**Test PUT update news (requires JWT)**:
```bash
curl -X PUT "https://localhost:5001/api/news/{news-id}" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "caption": "Updated Test News Article",
    "summary": "Updated summary"
  }'
```

**Expected Result**: 200 OK

**Test DELETE news (requires JWT)**:
```bash
curl -X DELETE "https://localhost:5001/api/news/{news-id}" \
  -H "Authorization: Bearer {jwt-token}"
```

**Expected Result**: 204 No Content

### Scenario 3: Input Validation

**Test invalid input validation**:
```bash
curl -X POST "https://localhost:5001/api/news" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "",
    "type": "news",
    "caption": "",
    "subjects": [],
    "authors": []
  }'
```

**Expected Result**: 400 Bad Request with validation error details

### Scenario 4: Security Headers

**Test security headers response**:
```bash
curl -I "https://localhost:5001/api/news"
```

**Expected Headers**:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`

### Scenario 5: CORS Configuration

**Test CORS preflight from allowed origin**:
```bash
curl -X OPTIONS "https://localhost:5001/api/news" \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: GET"
```

**Expected Result**: 200 OK with appropriate CORS headers

## Performance Verification

### Scenario 6: Caching Performance

**Test cached response performance**:
```bash
# First request (cache miss)
time curl -X GET "https://localhost:5001/api/news"

# Second request (cache hit - should be faster)
time curl -X GET "https://localhost:5001/api/news"
```

**Expected Result**: Second request should complete in <200ms

## Health Check

**Test application health**:
```bash
curl -X GET "https://localhost:5001/health"
```

**Expected Result**: 200 OK with health status including MongoDB connectivity

## Error Handling

### Scenario 7: Database Connection Failure

**Simulate MongoDB disconnection**:
1. Stop MongoDB service
2. Make API request
3. Observe graceful error handling

**Expected Result**: 503 Service Unavailable with appropriate error message

## Architecture Verification

### Scenario 8: Clean Architecture Structure

**Verify folder organization**:
```bash
# Check Clean Architecture folders exist
ls newsApi/Domain/
ls newsApi/Application/
ls newsApi/Infrastructure/
ls newsApi/Presentation/
```

**Expected Result**: Each folder contains appropriate files according to Clean Architecture principles

## Migration Verification

### Scenario 9: Data Migration

**Verify existing data compatibility**:
1. Run data migration script
2. Test CRUD operations on migrated data
3. Verify data integrity

**Expected Result**: All existing news articles accessible through new API structure

## Completion Criteria

- [ ] All public endpoints respond without authentication
- [ ] Protected endpoints require valid JWT tokens
- [ ] Input validation works with FluentValidation
- [ ] Security headers are present in all responses
- [ ] CORS works for configured origins
- [ ] Caching improves response times
- [ ] Health checks indicate system status
- [ ] Clean Architecture folders are properly organized
- [ ] Existing data is successfully migrated
- [ ] All scenarios complete successfully