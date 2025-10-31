# Contributing to News Portal

Thank you for your interest in contributing to News Portal! This guide will help you get started.

## 🚀 Quick Start

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/YOUR_USERNAME/newsportal.git`
3. **Create a branch**: `git checkout -b feature/your-feature-name`
4. **Make your changes** following our guidelines
5. **Test** your changes: `dotnet test`
6. **Commit** with clear messages
7. **Push** and create a **Pull Request**

## 📋 Prerequisites

Before contributing, ensure you have:

- **.NET 9 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop/))
- **Node.js 18+** for frontend (if contributing to frontend)
- **Git** for version control
- **VS Code** or **Visual Studio** (recommended)

## 🏗️ Development Setup

### 1. Set Up Backend

```bash
# Clone and navigate
cd newsportal

# Copy environment file
cp .env.example .env

# Start services with Docker
docker compose up -d

# Verify backend is running
curl http://localhost:5000/health
```

### 2. Run Tests

```bash
# Run all tests
dotnet test

# Run specific category
dotnet test --filter "FullyQualifiedName~Unit"
```

### 3. Set Up Frontend (Optional)

```bash
cd frontend
npm install
npm run dev
```

## 📝 Code Guidelines

### Clean Architecture

Follow the established architecture:

```
Domain → Application → Infrastructure → Presentation
```

- **Domain**: Pure business entities (no dependencies)
- **Application**: Business logic, DTOs, validators
- **Infrastructure**: Data access, external services
- **Presentation**: Controllers, middleware

### C# Coding Standards

- Use **PascalCase** for classes, methods, properties
- Use **camelCase** for local variables, parameters
- Use `_camelCase` for private fields
- Always use `async`/`await` for I/O operations
- Add XML documentation for public APIs
- Follow SOLID principles

**Example:**
```csharp
/// <summary>
/// Retrieves a news article by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the news article.</param>
/// <returns>The news article if found; otherwise, null.</returns>
public async Task<News?> GetByIdAsync(string id)
{
    _logger.LogInformation("GetByIdAsync called with id: {Id}", id);
    
    var news = await _repository.GetByIdAsync(id);
    
    if (news == null)
    {
        _logger.LogWarning("News not found: {Id}", id);
    }
    
    return news;
}
```

### Testing Requirements

Every change must include tests:

- **Unit tests** for business logic (services, validators)
- **Integration tests** for controllers
- **AAA pattern**: Arrange, Act, Assert
- **Test naming**: `MethodName_Scenario_ExpectedBehavior`

**Example:**
```csharp
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsNews()
{
    // Arrange
    var expectedNews = new News { Id = "123", Caption = "Test" };
    _mockRepository.Setup(r => r.GetByIdAsync("123"))
        .ReturnsAsync(expectedNews);

    // Act
    var result = await _service.GetByIdAsync("123");

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Caption);
}
```

## 🔄 Pull Request Process

### Before Submitting

- [ ] All tests pass (`dotnet test`)
- [ ] Code follows style guidelines
- [ ] New features have tests
- [ ] Documentation is updated
- [ ] No merge conflicts with master
- [ ] Commit messages are clear

### PR Title Format

Use conventional commits:

```
feat: Add search endpoint for news articles
fix: Resolve null reference in Category validator
docs: Update API documentation
chore: Update dependencies
test: Add integration tests for NewsController
```

### PR Description Template

```markdown
## Description
Brief description of what this PR does.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows project guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
```

## 🐛 Reporting Bugs

### Before Reporting

1. Check [existing issues](https://github.com/bkalafat/newsportal/issues)
2. Ensure you're using the latest version
3. Try to reproduce the issue

### Bug Report Template

```markdown
**Describe the bug**
Clear and concise description.

**To Reproduce**
Steps to reproduce:
1. Go to '...'
2. Click on '....'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 9.0]
- Docker Version: [e.g., 24.0.0]

**Additional context**
Any other context about the problem.
```

## 💡 Suggesting Features

### Feature Request Template

```markdown
**Is your feature related to a problem?**
Description of the problem.

**Describe the solution**
Clear description of what you want to happen.

**Describe alternatives**
Alternative solutions you've considered.

**Additional context**
Mockups, examples, or other context.
```

## 📖 Documentation

### When to Update Documentation

Update documentation when:
- Adding new features
- Changing API endpoints
- Modifying configuration
- Updating dependencies

### Documentation Locations

- **API docs**: Update Swagger annotations
- **Architecture**: `docs/ARCHITECTURE.md`
- **Build guide**: `docs/BUILD.md`
- **Run guide**: `docs/RUN.md`
- **Deploy guide**: `docs/DEPLOY.md`

## 🎯 Areas for Contribution

### Good First Issues

Look for issues tagged with:
- `good first issue` - Great for newcomers
- `help wanted` - We need help with this
- `documentation` - Improve docs

### Priority Areas

- **Testing**: Increase test coverage
- **Performance**: Optimize queries, caching
- **Security**: Fix vulnerabilities
- **Documentation**: Improve guides
- **Features**: See roadmap in issues

## 🔒 Security

If you discover a security vulnerability:

1. **DO NOT** open a public issue
2. Email: (Add security contact email)
3. Include:
   - Description of vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

## 📚 Resources

### Documentation
- [BUILD.md](docs/BUILD.md) - Build instructions
- [RUN.md](docs/RUN.md) - Running and development
- [DEPLOY.md](docs/DEPLOY.md) - Deployment guide
- [ARCHITECTURE.md](docs/ARCHITECTURE.md) - System architecture

### Coding Standards
- [C# Instructions](.github/instructions/csharp.instructions.md)
- [Testing Instructions](.github/instructions/testing.instructions.md)
- [API Controller Instructions](.github/instructions/api-controllers.instructions.md)

### Development Prompts
- [Add Endpoint](.github/prompts/add-endpoint.prompt.md)
- [Add Tests](.github/prompts/add-tests.prompt.md)
- [Debug Issues](.github/prompts/debug-issues.prompt.md)

## 🤝 Code of Conduct

### Our Standards

- Be respectful and inclusive
- Welcome diverse perspectives
- Focus on what's best for the community
- Show empathy towards others
- Accept constructive criticism gracefully

### Unacceptable Behavior

- Harassment or discriminatory language
- Personal attacks
- Publishing others' private information
- Unprofessional conduct

## 🎉 Recognition

Contributors will be:
- Listed in release notes
- Mentioned in GitHub contributors
- Credited in relevant documentation

## 📞 Getting Help

- **GitHub Discussions**: Ask questions and share ideas
- **Issues**: Report bugs or request features
- **Pull Requests**: Contribute code

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing to News Portal! 🚀**

Your contributions make this project better for everyone.
