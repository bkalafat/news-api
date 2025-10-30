# News Portal

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-16-black?logo=next.js)](https://nextjs.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A modern full-stack news platform built with .NET 9 and Next.js 16. Features Clean Architecture, JWT authentication, MongoDB persistence, and comprehensive caching for high performance.

## Features

- **Clean Architecture** - Domain-driven design with clear separation of concerns
- **RESTful API** - Built with ASP.NET Core 9 and Minimal APIs
- **JWT Authentication** - Secure token-based authentication
- **MongoDB** - NoSQL database with optimized queries
- **Memory Caching** - High-performance caching layer
- **Docker Ready** - Full containerization with Docker Compose
- **Modern Frontend** - Next.js 16 with TypeScript and Tailwind CSS
- **API Documentation** - Interactive Swagger/OpenAPI documentation
- **Comprehensive Tests** - Unit, integration, and performance tests

## Quick Start

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- OR [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) + [MongoDB](https://www.mongodb.com/try/download/community)

### Using Docker (Recommended)

```powershell
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f newsapi

# Stop services
docker-compose down
```

Access the application:
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **MongoDB UI**: http://localhost:8081 (admin/admin123)
- **MinIO Console**: http://localhost:9001 (minioadmin/minioadmin123)

### Manual Setup

```bash
# Clone repository
git clone https://github.com/bkalafat/newsportal.git
cd newsportal

# Configure secrets
cd backend
dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "DatabaseSettings:DatabaseName" "NewsDb"
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-min-32-chars"

# Run application
dotnet run --project newsApi.csproj
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/news` | List all news articles | No |
| GET | `/api/news/{id}` | Get article by ID | No |
| GET | `/api/news/by-url?url={slug}` | Get article by slug | No |
| POST | `/api/news` | Create new article | Yes |
| PUT | `/api/news/{id}` | Update article | Yes |
| DELETE | `/api/news/{id}` | Delete article | Yes |
| GET | `/health` | Health check | No |

See [API Documentation](NEWS_API_DOCUMENTATION.md) for detailed specs.

## Project Structure

```
newsportal/
├── backend/              # .NET 9 API
│   ├── Domain/          # Business entities
│   ├── Application/     # Use cases & DTOs
│   ├── Infrastructure/  # Data access & services
│   └── Presentation/    # Controllers & middleware
├── frontend/            # Next.js 16 app
├── tests/               # Test suite
└── docker/              # Docker configurations
```

## Development

```bash
# Run tests
dotnet test

# Run specific test category
dotnet test --filter "FullyQualifiedName~Unit"

# Build Docker image
docker build -f docker/Dockerfile.backend -t newsportal:latest .

# Frontend development
cd frontend
npm install
npm run dev
```

## Configuration

Key configuration options (use environment variables or User Secrets):

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "NewsDb"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "ExpirationMinutes": 60
  }
}
```

## Documentation

- [API Documentation](NEWS_API_DOCUMENTATION.md)
- [Swagger Testing Guide](SWAGGER_TESTING_GUIDE.md)
- [Frontend Documentation](frontend/README.md)
- [Architecture Specs](specs/002-modernize-net-core/)

## Deployment

**Azure Container Apps**: Deployed at [newsportal-backend.agreeableglacier-cfb21c4c.eastus.azurecontainerapps.io](https://newsportal-backend.agreeableglacier-cfb21c4c.eastus.azurecontainerapps.io)

**Netlify**: Frontend deployed automatically from `master` branch

See [Azure Deployment Guide](AZURE_DEPLOYMENT.md) for details.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

## Contact

- Repository: [github.com/bkalafat/newsportal](https://github.com/bkalafat/newsportal)
- Issues: [github.com/bkalafat/newsportal/issues](https://github.com/bkalafat/newsportal/issues)
