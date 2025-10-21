# Contributing to News API

Thank you for your interest in contributing to the News API project! This document provides guidelines and instructions for contributing.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing Requirements](#testing-requirements)
- [Pull Request Process](#pull-request-process)
- [Documentation](#documentation)

## 🤝 Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inspiring community for all. Please be respectful and constructive in all interactions.

### Expected Behavior

- Use welcoming and inclusive language
- Be respectful of differing viewpoints
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud)
- [Git](https://git-scm.com/)
- IDE: [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension

### Initial Setup

1. **Fork the repository**
   ```bash
   # Click the "Fork" button on GitHub
   ```

2. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR_USERNAME/news-api.git
   cd news-api
   ```

3. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/bkalafat/news-api.git
   ```

4. **Configure User Secrets**
   ```bash
   cd newsApi
   dotnet user-secrets init
   dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://localhost:27017"
   dotnet user-secrets set "DatabaseSettings:DatabaseName" "NewsDb"
   dotnet user-secrets set "JwtSettings:SecretKey" "your-dev-secret-key-min-32-chars"
   ```

5. **Restore dependencies**
   ```bash
   dotnet restore
   ```

6. **Run tests**
   ```bash
   dotnet test
   ```

7. **Run the application**
   ```bash
   dotnet run --project newsApi/newsApi.csproj
   ```

## 🔄 Development Workflow

### 1. Create a Feature Branch

```bash
# Sync with upstream
git checkout main
git pull upstream main

# Create feature branch
git checkout -b feature/your-feature-name
```

### Branch Naming Convention

- **Features**: `feature/short-description`
- **Bug fixes**: `fix/issue-number-description`
- **Documentation**: `docs/what-is-documented`
- **Refactoring**: `refactor/what-is-refactored`

### 2. Make Your Changes

Follow the coding standards and architectural principles:
- Clean Architecture layer separation
- SOLID principles
- DRY (Don't Repeat Yourself)
- See `.github/instructions/` for detailed guidelines

### 3. Write Tests

All code changes must include tests:
- **Unit tests** for business logic
- **Integration tests** for API endpoints
- Follow AAA pattern (Arrange-Act-Assert)
- See `.github/instructions/testing.instructions.md`

### 4. Update Documentation

- Add/update XML comments for public APIs
- Update README.md if adding features
- Update NEWS_API_DOCUMENTATION.md for API changes
- Add examples for complex features

### 5. Commit Your Changes

```bash
git add .
git commit -m "feat: add search functionality for news articles"
```

#### Commit Message Convention

Format: `type: description`

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat: add pagination to news endpoint
fix: resolve null reference in NewsService
docs: update API documentation for search endpoint
test: add integration tests for NewsController
refactor: simplify validation logic in CreateNewsDto
```

### 6. Push to Your Fork

```bash
git push origin feature/your-feature-name
```

### 7. Create Pull Request

1. Go to your fork on GitHub
2. Click "New Pull Request"
3. Select `base: main` ← `compare: feature/your-feature-name`
4. Fill in the PR template
5. Link related issues

## 🎨 Coding Standards

### C# Guidelines

Follow Microsoft C# conventions:
- PascalCase for classes, methods, properties
- camelCase for variables, parameters
- _camelCase for private fields
- Use `var` when type is obvious
- Use async/await for I/O operations
- See `.github/instructions/csharp.instructions.md`

### Architecture Principles

**Clean Architecture:**
```
Domain/         → No external dependencies
Application/    → Depends only on Domain
Infrastructure/ → Implements Application interfaces
Presentation/   → Depends on Application
```

**Dependency Rules:**
- Dependencies point inward
- Domain has no dependencies
- Application depends only on Domain
- Infrastructure implements Application interfaces
- Presentation depends on Application (not Infrastructure)

### Code Quality

- **No warnings**: Code must compile without warnings
- **No commented code**: Remove commented-out code
- **No magic numbers**: Use named constants
- **No hardcoded strings**: Use configuration or constants
- **Single Responsibility**: Each class/method does one thing
- **DRY**: Don't Repeat Yourself

## 🧪 Testing Requirements

### Test Coverage

- **Minimum**: 80% code coverage
- **Critical paths**: 100% coverage for business logic

### Test Types

1. **Unit Tests** (`NewsApi.Tests/Unit/`)
   - Test individual components in isolation
   - Use Moq for mocking
   - Fast execution (< 100ms each)

2. **Integration Tests** (`NewsApi.Tests/Integration/`)
   - Test components working together
   - Use WebApplicationFactory
   - Test all API endpoints

3. **Validator Tests** (`NewsApi.Tests/Unit/Validators/`)
   - Test all validation rules
   - Test edge cases

### Running Tests

```bash
# All tests
dotnet test

# Specific category
dotnet test --filter "FullyQualifiedName~Unit"

# With coverage
dotnet test /p:CollectCoverage=true

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Test Requirements

- ✅ All public methods tested
- ✅ Happy path covered
- ✅ Error scenarios tested
- ✅ Edge cases handled
- ✅ Tests pass consistently
- ✅ No flaky tests
- ✅ Tests are independent
- ✅ Follow AAA pattern

## 📝 Pull Request Process

### Before Submitting

- [ ] Code compiles without warnings
- [ ] All tests pass
- [ ] New tests added for new functionality
- [ ] Documentation updated
- [ ] Commit messages follow convention
- [ ] Code follows project standards
- [ ] No merge conflicts with main

### PR Checklist

Your PR description should include:

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] All tests pass locally

## Documentation
- [ ] XML comments added
- [ ] README.md updated (if needed)
- [ ] API docs updated (if needed)

## Related Issues
Closes #123
```

### Review Process

1. Automated checks run (build, tests)
2. Code review by maintainers
3. Address feedback
4. Get approval
5. Squash and merge

### After Merge

Your feature branch will be deleted automatically.

## 📖 Documentation

### Code Documentation

**XML Comments** for all public APIs:
```csharp
/// <summary>
/// Retrieves a news article by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the news article.</param>
/// <returns>The news article if found; otherwise, null.</returns>
/// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
public async Task<News?> GetByIdAsync(string id)
{
    // Implementation
}
```

### Project Documentation

Update these files when relevant:
- `README.md` - Project overview and setup
- `NEWS_API_DOCUMENTATION.md` - API reference
- `SWAGGER_TESTING_GUIDE.md` - Testing guide
- `.github/instructions/` - Development guidelines

## 🐛 Reporting Issues

### Before Reporting

1. Check existing issues
2. Search closed issues
3. Try latest version
4. Verify it's not a configuration issue

### Issue Template

```markdown
**Description**
Clear description of the issue

**Steps to Reproduce**
1. Step 1
2. Step 2
3. Step 3

**Expected Behavior**
What should happen

**Actual Behavior**
What actually happens

**Environment**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 10.0]
- MongoDB Version: [e.g., 7.0]

**Additional Context**
Logs, screenshots, etc.
```

## 💡 Feature Requests

We welcome feature suggestions! Please:
1. Check if it aligns with project goals
2. Provide clear use cases
3. Be open to discussion
4. Consider implementing it yourself

## ❓ Questions

- **Documentation**: Check `.github/instructions/` and project docs
- **GitHub Discussions**: Ask questions in Discussions
- **Issues**: For bugs and feature requests only

## 📚 Additional Resources

- [Clean Architecture Guide](https://github.com/jasontaylordev/CleanArchitecture)
- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [xUnit Documentation](https://xunit.net/)

## 🙏 Thank You

Thank you for contributing to News API! Your efforts help make this project better for everyone.

---

**Questions?** Open an issue or start a discussion on GitHub.
