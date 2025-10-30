# 📰 News API - Full-Stack News Platform

[![.NET Version](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-16-black)](https://nextjs.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-3.2-green)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A modern, full-stack Turkish technology news platform. Features a production-ready .NET 9 backend API with Clean Architecture and a performant Next.js 16 frontend optimized for SEO and user experience.

## 🌟 Platform Components

### Backend API (backend/)
Modern news management API built with .NET 9, following Clean Architecture principles. Features JWT authentication, comprehensive caching, and MongoDB persistence.

### Frontend Web (frontend/)
Modern, SEO-optimized Turkish tech news website built with Next.js 16, TypeScript, TailwindCSS, and Shadcn/ui. Features responsive design, React Query data management, and Turkish localization.

## ✨ Features

- **Clean Architecture** - Clear separation of concerns with Domain, Application, Infrastructure, and Presentation layers
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

- **Docker Desktop** (Windows/Mac) or **Docker Engine** (Linux) - [Download](https://www.docker.com/products/docker-desktop/)
- **OR** Manual setup:
  - [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
  - [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud instance)

### 🐳 Docker Setup (Recommended)

The easiest way to run the entire stack locally is using Docker Compose.

#### 1. Start All Services

```powershell
# Copy environment template
Copy-Item .env.example .env

# Start services (builds images if needed)
.\docker-start.ps1 -Build

# Or use docker-compose directly
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

#### 2. Access Services

| Service | URL | Credentials |
|---------|-----|-------------|
| **News API** | http://localhost:5000 | N/A |
| **Swagger UI** | http://localhost:5000/swagger | N/A |
| **MinIO Console** | http://localhost:9001 | `minioadmin` / `minioadmin123` |
| **Mongo Express** | http://localhost:8081 | `admin` / `admin123` |
| **MongoDB** | `mongodb://localhost:27017` | `admin` / `password123` |

#### 3. Helper Scripts

```powershell
# View logs
.\docker-logs.ps1 newsapi -Follow

# Check service status
.\docker-status.ps1

# Rebuild after code changes
.\docker-rebuild.ps1

# Stop services
.\docker-stop.ps1

# Clean everything (removes data!)
.\docker-clean.ps1
```

For complete Docker documentation, see [DOCKER_GUIDE.md](DOCKER_GUIDE.md).

### 💻 Manual Setup

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
   git clone https://github.com/bkalafat/newsportal.git
   cd newsportal
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
newsportal/                   # Root monorepo
├── backend/                  # Backend (.NET 9 API)
│   ├── Domain/              # Core business logic & entities
│   ├── Application/         # Business rules & use cases
│   ├── Infrastructure/      # External dependencies
│   └── Presentation/        # API controllers & middleware
├── tests/                   # Backend test suite
│   ├── Unit/               # Unit tests
│   ├── Integration/        # Integration tests
│   └── Performance/        # Performance tests
├── frontend/                # Frontend (Next.js 16)
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
docker build -f Dockerfile.backend -t newsportal:latest ..
```

### Run Container
```bash
docker run -d -p 5000:8080 \
  -e DatabaseSettings__ConnectionString="mongodb://host.docker.internal:27017" \
  -e DatabaseSettings__DatabaseName="NewsDb" \
  -e JwtSettings__SecretKey="your-secret-key" \
  --name newsportal \
  newsportal:latest
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

- Built with [.NET 9](https://dotnet.microsoft.com/)
- MongoDB data persistence
- Swagger documentation via [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## 📧 Contact & Support

- **Repository**: [github.com/bkalafat/newsportal](https://github.com/bkalafat/newsportal)
- **Issues**: [github.com/bkalafat/newsportal/issues](https://github.com/bkalafat/newsportal/issues)
- **Email**: support@newsapi.com

---

**Built with ❤️ using Clean Architecture and modern .NET practices**
