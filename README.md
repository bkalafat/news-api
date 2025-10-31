# News Portal

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-16-black?logo=next.js)](https://nextjs.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-178%20Passing-success)](tests/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> A professional full-stack news platform with Clean Architecture, JWT authentication, and modern web technologies.

## 🚀 Quick Start

```bash
# 1. Clone and setup
git clone https://github.com/bkalafat/newsportal.git
cd newsportal
cp .env.example .env

# 2. Start with Docker (recommended)
docker compose up -d

# 3. Access services
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
# MongoDB UI: http://localhost:8081 (admin/admin123)
# MinIO Console: http://localhost:9001 (minioadmin/minioadmin123)
```

## ✨ Key Features

- **Clean Architecture** - Domain-driven design with SOLID principles
- **RESTful API** - ASP.NET Core 9 with comprehensive validation
- **JWT Authentication** - Secure token-based authorization
- **MongoDB** - Flexible NoSQL storage with optimized indexes
- **MinIO Storage** - S3-compatible object storage for images
- **Memory Caching** - High-performance in-memory caching
- **Modern Frontend** - Next.js 16, TypeScript, TailwindCSS
- **Auto-Scaling** - Production deployment on Azure Container Apps
- **178+ Tests** - Comprehensive unit and integration test coverage

## 📁 Project Structure

```
newsportal/
├── backend/                 # .NET 9 Backend API
│   ├── Domain/             # Core business entities (News, ImageMetadata)
│   ├── Application/        # Business logic, services, DTOs, validators
│   ├── Infrastructure/     # MongoDB, MinIO, JWT, caching
│   ├── Presentation/       # Controllers, middleware, extensions
│   └── Common/             # Shared utilities (SlugHelper for Turkish)
├── frontend/               # Next.js 16 Frontend
│   ├── app/               # App Router pages
│   ├── components/        # React components (Shadcn/ui)
│   └── lib/               # API client, utilities, hooks
├── tests/                 # Test suite (178+ tests)
│   ├── Unit/             # Service, validator, DTO tests
│   └── Integration/      # Controller, repository tests
├── docs/                 # Documentation
│   ├── BUILD.md         # Build instructions
│   ├── RUN.md           # Running locally & development
│   ├── DEPLOY.md        # Production deployment guide
│   └── ARCHITECTURE.md  # Architecture & design patterns
├── .github/              # GitHub Actions workflows
└── docker-compose.yml   # Docker orchestration
```

## 📖 Documentation

| Document | Description |
|----------|-------------|
| **[BUILD.md](docs/BUILD.md)** | How to build the project (Docker & local) |
| **[RUN.md](docs/RUN.md)** | Running & development guide |
| **[DEPLOY.md](docs/DEPLOY.md)** | Production deployment instructions |
| **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** | System architecture & design patterns |
| **[Frontend README](frontend/README.md)** | Next.js frontend documentation |
| **[Azure README](azure/README.md)** | Azure deployment details |

## 🔧 Development

### Run Tests

```bash
# All tests (178+)
dotnet test newsApi.sln

# Unit tests only
dotnet test --filter "FullyQualifiedName~Unit"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"
```

### Docker Helper Scripts (Windows)

```powershell
.\docker-start.ps1     # Start all services
.\docker-stop.ps1      # Stop all services
.\docker-logs.ps1      # View logs
.\docker-status.ps1    # Check container status
.\docker-rebuild.ps1   # Rebuild and restart
```

### Hot Reload Development

```bash
# Backend (watches for code changes)
cd backend
dotnet watch run

# Frontend (Next.js dev server)
cd frontend
npm run dev
```

## 🌐 API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/news` | List all news articles | No |
| GET | `/api/news/{id}` | Get article by ID | No |
| GET | `/api/news/by-url?url={slug}` | Get article by slug | No |
| POST | `/api/news` | Create new article | ✅ JWT |
| PUT | `/api/news/{id}` | Update article | ✅ JWT |
| DELETE | `/api/news/{id}` | Delete article | ✅ JWT |
| POST | `/api/news/upload-image` | Upload image | ✅ JWT |
| POST | `/api/auth/login` | Get JWT token | No |
| GET | `/health` | Health check | No |

**Interactive API docs**: http://localhost:5000/swagger

## 🚢 Deployment

### Production Stack

- **Backend**: Azure Container Apps (auto-scaling, 0-10 replicas)
- **Frontend**: Vercel / Azure Static Web Apps
- **Database**: MongoDB Atlas (M0 free tier)
- **Storage**: MinIO / Cloudflare R2
- **CI/CD**: GitHub Actions (auto-deploy on push to master)

### Production URLs

- **Backend API**: `https://newsportal-backend.*.azurecontainerapps.io`
- **Swagger**: `https://newsportal-backend.*.azurecontainerapps.io/swagger`
- **Frontend**: Deployed via Vercel

See **[DEPLOY.md](docs/DEPLOY.md)** for complete deployment guide.

## 🛠️ Tech Stack

### Backend
- **.NET 9** - Latest C# 13 with minimal APIs
- **MongoDB 7.0** - Flexible NoSQL document database
- **MinIO** - S3-compatible object storage
- **JWT Bearer** - Token-based authentication
- **FluentValidation** - Request validation
- **xUnit + Moq** - Comprehensive testing

### Frontend
- **Next.js 16** - React framework with App Router
- **TypeScript 5** - Type-safe development
- **TailwindCSS 4** - Modern utility-first CSS
- **Shadcn/ui** - Beautiful accessible components
- **React Query** - Powerful data fetching
- **next-intl** - Turkish localization

### Infrastructure
- **Docker Compose** - Local development environment
- **Azure Container Apps** - Production backend hosting
- **Vercel** - Frontend CDN deployment
- **GitHub Actions** - CI/CD automation

## 📋 Prerequisites

- **Docker Desktop** (recommended) OR
- **.NET 9 SDK** + **MongoDB** + **Node.js 18+**

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure:
- All tests pass (`dotnet test`)
- Code follows existing patterns
- Documentation is updated

## 📄 License

This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/bkalafat/newsportal/issues)
- **Discussions**: [GitHub Discussions](https://github.com/bkalafat/newsportal/discussions)
- **Repository**: [github.com/bkalafat/newsportal](https://github.com/bkalafat/newsportal)

---

**Built with ❤️ using Clean Architecture principles**
