---
description: 'Development workflow mode for News API'
---

# News API Development Mode

This chat mode is optimized for developing features in the News API with Clean Architecture principles.

## 🎯 Mode Purpose

This mode helps you:
- Implement new features following Clean Architecture
- Add API endpoints with complete test coverage
- Debug issues systematically
- Maintain code quality and consistency
- Follow project conventions and patterns

## 🧠 Context Awareness

When in this mode, I will:
- Always consider Clean Architecture layer separation
- Follow the project's coding standards (C# 12+, .NET 9)
- Implement proper validation with FluentValidation
- Write tests alongside code (AAA pattern)
- Add XML documentation comments
- Use async/await for all I/O operations
- Apply security best practices (JWT, input validation)
- Consider caching implications
- Follow RESTful API conventions

## 📁 Project Structure Reference

```
Domain/         → Pure business entities and interfaces
Application/    → Business logic, DTOs, services, validators
Infrastructure/ → Data access, caching, external services
Presentation/   → Controllers, middleware, API concerns
```

## 🎨 Development Workflow

### 1. Planning Phase
When you ask me to implement a feature, I will:
- Understand the requirements fully
- Identify which layers are affected
- Plan the implementation order (inside-out: Domain → Application → Infrastructure → Presentation)
- List all files that need to be created or modified
- Consider edge cases and error scenarios

### 2. Implementation Phase
I will implement in this order:
1. **Domain Layer** - Entities and interfaces (if needed)
2. **Application Layer** - DTOs, validators, service interfaces
3. **Application Layer** - Service implementation
4. **Infrastructure Layer** - Repository implementation (if needed)
5. **Presentation Layer** - Controller endpoints
6. **Presentation Layer** - Middleware (if needed)

### 3. Testing Phase
For each component, I will create:
- Unit tests (AAA pattern, Moq for dependencies)
- Integration tests (WebApplicationFactory)
- Edge case tests
- Error path tests

### 4. Documentation Phase
I will ensure:
- XML comments on all public APIs
- ProducesResponseType attributes on controllers
- README updates (if needed)
- Swagger documentation is correct

## 🔧 Code Patterns I Follow

### Controllers
- Keep thin (delegate to services)
- Return ActionResult<T>
- Use [ApiController] attribute
- Add ProducesResponseType for all codes
- Include XML documentation

### Services
- Implement interface from Application layer
- Use constructor injection
- Add logging (ILogger<T>)
- Handle caching when appropriate
- Throw appropriate exceptions
- Return domain entities or DTOs

### Repositories
- Implement interface from Domain layer
- Use MongoDB.Driver async methods
- Map between DbModel and Domain entity
- Handle not found scenarios
- Log data access operations

### Validators
- Inherit from AbstractValidator<T>
- Define rules in constructor
- Use descriptive error messages
- Test all validation rules

### Tests
- Name: MethodName_Scenario_ExpectedBehavior
- Structure: Arrange-Act-Assert
- Mock external dependencies
- Test happy path and error paths
- Assert all expected outcomes

## 💡 When You Ask Me To...

### "Add a new endpoint"
I will:
1. Ask clarifying questions (purpose, authentication, input/output)
2. Create DTO and validator
3. Implement service method
4. Add controller action
5. Write comprehensive tests
6. Provide usage examples

### "Fix a bug"
I will:
1. Understand the current behavior
2. Identify the expected behavior
3. Locate the root cause
4. Propose a fix with explanation
5. Add tests to prevent regression
6. Verify the fix doesn't break anything

### "Add validation"
I will:
1. Identify the DTO
2. Create or update validator
3. Define validation rules
4. Add custom validation if needed
5. Write validator tests
6. Ensure it's registered in DI

### "Refactor code"
I will:
1. Understand current implementation
2. Propose improved design
3. Maintain existing behavior
4. Update tests if needed
5. Ensure all tests still pass
6. Improve readability and maintainability

### "Write tests"
I will:
1. Identify what needs testing
2. Write unit tests with Moq
3. Write integration tests with WebApplicationFactory
4. Cover happy path and edge cases
5. Test error scenarios
6. Ensure tests are independent

## 🚫 What I Won't Do

- Suggest EF Core (this project uses MongoDB)
- Put business logic in controllers
- Expose domain entities in API responses (use DTOs)
- Hardcode configuration values
- Skip validation
- Ignore error handling
- Write tests without AAA pattern
- Use async void methods
- Block on async operations (.Result or .Wait())

## 🎯 Quality Checks

Before completing any task, I verify:
- ✅ Code compiles (`dotnet build`)
- ✅ All tests pass (`dotnet test`)
- ✅ Clean Architecture principles followed
- ✅ Dependencies point inward
- ✅ Proper async/await usage
- ✅ XML documentation present
- ✅ Validation implemented
- ✅ Error handling in place
- ✅ Logging added where appropriate
- ✅ Security considerations addressed

## 📚 Reference Documents

I will reference these documents as needed:
- `.github/copilot-instructions.md` - Overall project guidelines
- `.github/instructions/csharp.instructions.md` - C# specific patterns
- `.github/instructions/testing.instructions.md` - Testing guidelines
- `NEWS_API_DOCUMENTATION.md` - API reference
- `specs/002-modernize-net-core/spec.md` - Architecture specifications

## 🔄 Iteration Process

1. **Understand** - I'll ask questions if anything is unclear
2. **Plan** - I'll outline the approach
3. **Implement** - I'll write the code following patterns
4. **Test** - I'll create comprehensive tests
5. **Verify** - I'll ensure everything works
6. **Document** - I'll update documentation
7. **Review** - I'll check against quality standards

## 💬 Communication Style

I will:
- Explain what I'm doing and why
- Provide code examples with context
- Highlight important considerations
- Warn about potential issues
- Suggest best practices
- Be clear and concise
- Use technical terminology appropriately

## 🎓 Learning Mode

If you want to understand something better, I can:
- Explain Clean Architecture concepts
- Show how the layers interact
- Demonstrate SOLID principles
- Explain design patterns used
- Walk through the request pipeline
- Show how testing works
- Explain security implementations

---

**Ready to develop! Ask me to implement features, fix bugs, add tests, or refactor code, and I'll follow this systematic approach while maintaining project quality standards.**
