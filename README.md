# 📰 News API - Full-Stack News Platform

[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15-black)](https://nextjs.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-Latest-green)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A modern, full-stack Turkish technology news platform. Features a production-ready .NET 10 backend API with Clean Architecture and a performant Next.js 15 frontend optimized for SEO and user experience.

## 🌟 Platform Components

### Backend API (backend/)
Modern news management API built with .NET 10, following Clean Architecture principles. Features real-time RSS integration with BBC News feeds, JWT authentication, comprehensive caching, and MongoDB persistence.

### Frontend Web (frontend/)
Modern, SEO-optimized Turkish tech news website built with Next.js 15, TypeScript, TailwindCSS, and Shadcn/ui. Features responsive design, React Query data management, and Turkish localization.

## ✨ Features

- **Clean Architecture** - Clear separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- **RSS Integration** - Automatic news fetching from BBC RSS feeds across multiple categories
- **JWT Authentication** - Secure API endpoints with OAuth2/OpenID Connect standards
- **MongoDB Persistence** - Flexible NoSQL data storage with optimized queries
- **Memory Caching** - High-performance caching layer with configurable TTL
- **Input Validation** - FluentValidation framework with comprehensive rules
- **Security Hardened** - Protection against OWASP Top 10 vulnerabilities
- **Health Checks** - MongoDB connectivity monitoring
- **Swagger Documentation** - Interactive API documentation and testing
- **Docker Ready** - Containerized deployment with Docker and Heroku support
- **Comprehensive Testing** - Unit, integration, and performance test coverage

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud instance)
- [Docker](https://www.docker.com/) (optional, for containerized deployment)

### Installation

### Quick Start with Scripts

We provide convenient scripts for development:

```bash
# Start both backend and frontend (Windows)
scripts\dev.bat

# Start Docker containers (MongoDB + MinIO)
scripts\docker-start.bat

# Build both projects
scripts\build.bat
```

### Manual Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/bkalafat/news-api.git
   cd news-api
   ```

2. **Start Docker services** (MongoDB + MinIO)
   ```bash
   cd docker
   docker-compose up -d
   ```

2. **Configure User Secrets** (Development)
   ```bash
   cd backend
   dotnet user-secrets init
   dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://localhost:27017"
   dotnet user-secrets set "DatabaseSettings:DatabaseName" "NewsDb"
   dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-min-32-chars"
   ```

4. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the Application**
   ```bash
   dotnet run --project backend/newsApi.csproj
   ```

5. **Access Swagger UI**
   
   Navigate to: [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Frontend Setup (frontend/)

1. **Navigate to frontend folder**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   ```bash
   cp .env.example .env.local
   # Edit .env.local with your backend URL
   ```

4. **Run development server**
   ```bash
   npm run dev
   ```

5. **Access the website**
   
   Navigate to: [http://localhost:3000](http://localhost:3000)

**Frontend Documentation**: See [frontend/README.md](frontend/README.md) for complete frontend setup and development guide.

## 📚 API Documentation

### Base URL
- **Development**: `http://localhost:5000`
- **Production**: Your deployed URL

### Available Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/news` | Get all news with filtering | ❌ |
| `GET` | `/api/news/{id}` | Get news by ID | ❌ |
| `GET` | `/api/news/by-url?url={slug}` | Get news by URL slug | ❌ |
| `POST` | `/api/news` | Create new news article | ✅ |
| `PUT` | `/api/news/{id}` | Update existing news | ✅ |
| `DELETE` | `/api/news/{id}` | Delete news article | ✅ |
| `GET` | `/health` | Health check endpoint | ❌ |

**Detailed Documentation**: See [NEWS_API_DOCUMENTATION.md](NEWS_API_DOCUMENTATION.md) for complete API reference with request/response examples.

**Swagger Testing Guide**: See [SWAGGER_TESTING_GUIDE.md](SWAGGER_TESTING_GUIDE.md) for interactive testing instructions.

## 🏗️ Project Structure

This project follows modern **monorepo architecture** with clean separation:

```
news-api/                     # Root monorepo
├── backend/                  # Backend (.NET 10 API)
│   ├── Domain/              # Core business logic & entities
│   ├── Application/         # Business rules & use cases
│   ├── Infrastructure/      # External dependencies
│   └── Presentation/        # API controllers & middleware
├── tests/                   # Backend test suite
│   ├── Unit/               # Unit tests
│   ├── Integration/        # Integration tests
│   └── Performance/        # Performance tests
├── frontend/                # Frontend (Next.js 15)
│   ├── app/                # Next.js pages
│   ├── components/         # React components
│   ├── lib/                # API client & utilities
│   └── public/             # Static assets
├── docker/                  # Docker configurations
│   ├── docker-compose.yml  # Service orchestration
│   └── Dockerfile.backend  # Backend container
├── scripts/                 # Development scripts
│   ├── dev.bat             # Start dev servers
│   ├── build.bat           # Build all projects
│   └── docker-start.bat    # Start Docker services
├── .github/                 # GitHub configurations
│   ├── instructions/       # Copilot guidelines
│   └── prompts/            # Development prompts
└── README.md               # This file
```
└── Performance/             # Performance benchmarks
```

### Key Design Patterns

- **Repository Pattern** - Abstraction over data access
- **Dependency Injection** - Loose coupling and testability
- **CQRS Principles** - Clear separation of commands and queries
- **Middleware Pipeline** - Cross-cutting concerns handling
- **DTO Pattern** - API contract isolation from domain

## 🔧 Configuration

### Environment Variables

The API uses .NET User Secrets for local development and environment variables for production:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "NewsDb",
    "CollectionName": "news"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "NewsApi",
    "Audience": "NewsApiUsers",
    "ExpirationMinutes": 60
  },
  "CacheSettings": {
    "DefaultExpirationMinutes": 30
  }
}
```

### Categories Supported

- Technology
- World
- Business
- Science
- Health
- Entertainment

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName~Unit"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"

# Performance tests only
dotnet test --filter "FullyQualifiedName~Performance"
```

### Test Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Test Coverage Report**: See [TEST_COVERAGE_REPORT.md](NewsApi.Tests/TEST_COVERAGE_REPORT.md)

## 🐳 Docker Deployment

### Start Services with Docker

```bash
# Start MongoDB and MinIO
cd docker
docker-compose up -d
```

### Build Docker Image
```bash
cd docker
docker build -f Dockerfile.backend -t news-api:latest ..
```

### Run Container
```bash
docker run -d -p 5000:8080 \
  -e DatabaseSettings__ConnectionString="mongodb://host.docker.internal:27017" \
  -e DatabaseSettings__DatabaseName="NewsDb" \
  -e JwtSettings__SecretKey="your-secret-key" \
  --name news-api \
  news-api:latest
```

### Heroku Deployment
This project includes `heroku.yml` for automated Heroku deployment:
```bash
heroku container:push web -a your-app-name
heroku container:release web -a your-app-name
```

## 🔒 Security

### Authentication
- **JWT Bearer Tokens** - OAuth2/OpenID Connect compliant
- **Token Expiration** - Configurable token lifetime
- **Secure Headers** - HSTS, X-Content-Type-Options, X-Frame-Options

### Input Validation
- **FluentValidation** - Comprehensive validation rules
- **Server-side Validation** - All inputs validated before processing
- **XSS Protection** - Input sanitization and encoding

### OWASP Top 10 Protection
- SQL Injection: MongoDB parameterized queries
- XSS: Input encoding and validation
- CSRF: Token-based authentication
- Security misconfiguration: Secure defaults
- Sensitive data exposure: User Secrets & Azure Key Vault

## 📖 Additional Documentation

- **[NEWS_API_DOCUMENTATION.md](NEWS_API_DOCUMENTATION.md)** - Complete API reference
- **[SWAGGER_TESTING_GUIDE.md](SWAGGER_TESTING_GUIDE.md)** - Interactive testing guide
- **[specs/002-modernize-net-core/](specs/002-modernize-net-core/)** - Architecture specifications
- **[Migration Guide](Migration/data-migration.md)** - Data migration instructions

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the coding guidelines (see `.github/instructions/`)
4. Write tests for new functionality
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Coding Standards
- Follow Microsoft C# coding conventions
- Use C# 12+ language features
- Write XML documentation comments for public APIs
- Maintain test coverage above 80%
- Follow Clean Architecture principles

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [.NET 10](https://dotnet.microsoft.com/)
- RSS feeds powered by [BBC News](https://www.bbc.com/news)
- MongoDB data persistence
- Swagger documentation via [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## 📧 Contact & Support

- **Repository**: [github.com/bkalafat/news-api](https://github.com/bkalafat/news-api)
- **Issues**: [github.com/bkalafat/news-api/issues](https://github.com/bkalafat/news-api/issues)
- **Email**: support@newsapi.com

---

**Built with ❤️ using Clean Architecture and modern .NET practices**
