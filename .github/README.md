# GitHub Copilot Documentation Index

Welcome! This directory contains comprehensive GitHub Copilot instructions, prompts, and chatmodes to help you work effectively with the News API project.

## 📂 Directory Structure

```
.github/
├── copilot-instructions.md    # Main project instructions (always loaded)
├── instructions/               # Context-specific instruction files
│   ├── csharp.instructions.md
│   ├── testing.instructions.md
│   └── api-controllers.instructions.md
├── prompts/                    # Reusable task prompts
│   ├── add-endpoint.prompt.md
│   ├── add-tests.prompt.md
│   └── debug-issues.prompt.md
└── chatmodes/                  # Specialized chat modes
    └── development.chatmode.md
```

## 📖 Quick Start

### For New Contributors
1. Read `copilot-instructions.md` - Overall project context
2. Review `CONTRIBUTING.md` - Contribution guidelines
3. Check `README.md` - Project setup and features

### For Development
1. Activate `development.chatmode.md` - Development workflow mode
2. Use prompts in `prompts/` for common tasks
3. Reference instructions in `instructions/` for specific guidance

## 📋 Main Instructions File

### [copilot-instructions.md](copilot-instructions.md)
**Loaded automatically for every Copilot interaction**

Contains:
- 📋 Project overview and features
- 🛠️ Complete tech stack
- 📁 Project structure guide
- 🎯 Coding guidelines and standards
- 🔧 Available scripts and resources
- 🧩 Common development tasks
- 🚫 Anti-patterns to avoid
- 📚 Reference documentation

**When to reference**: Always! This is your primary context.

## 🎯 Instruction Files

Instruction files provide specialized guidance for specific file types or contexts.

### [instructions/csharp.instructions.md](instructions/csharp.instructions.md)
**Applies to**: All `*.cs` files

Contains:
- 🧠 C# 12+ language features
- 🔧 Naming conventions
- 📝 Code organization patterns
- 🔄 Async/await best practices
- 📊 LINQ guidelines
- 🎨 Dependency injection patterns
- 🧶 Repository and service patterns
- ✅ Do's and ❌ Don'ts

**Use when**: Writing or reviewing any C# code.

### [instructions/testing.instructions.md](instructions/testing.instructions.md)
**Applies to**: All files in `**/Tests/**/*.cs`

Contains:
- 🧪 xUnit testing framework usage
- 🎭 Moq mocking patterns
- 📝 AAA pattern (Arrange-Act-Assert)
- 🔧 Test data builders
- 🌐 Integration testing with WebApplicationFactory
- ✅ Test coverage requirements
- 📊 What to test and what not to test

**Use when**: Writing or debugging tests.

### [instructions/api-controllers.instructions.md](instructions/api-controllers.instructions.md)
**Applies to**: All files in `**/Controllers/**/*.cs`

Contains:
- 🎯 Controller design principles
- 📝 HTTP method patterns (GET, POST, PUT, DELETE)
- 🔒 Authorization patterns
- 📊 Parameter binding (route, query, body)
- ✅ Best practices
- 📚 RESTful API conventions
- 📖 Documentation requirements

**Use when**: Working with API controllers.

## 🎬 Prompt Files

Prompt files guide you through specific development tasks.

### [prompts/add-endpoint.prompt.md](prompts/add-endpoint.prompt.md)
**Use when**: Adding a new API endpoint

Guides you through:
1. Creating DTOs
2. Adding validators
3. Implementing service methods
4. Adding controller actions
5. Writing comprehensive tests
6. Testing via Swagger

**Example usage**: 
```
"Following the add-endpoint prompt, help me create a search endpoint"
```

### [prompts/add-tests.prompt.md](prompts/add-tests.prompt.md)
**Use when**: Adding tests for existing code

Provides:
- Test templates for services, validators, controllers
- Test data builder patterns
- Coverage analysis guidance
- Test scenario checklists

**Example usage**:
```
"Using the add-tests prompt, help me add tests for NewsService"
```

### [prompts/debug-issues.prompt.md](prompts/debug-issues.prompt.md)
**Use when**: Debugging problems

Covers:
- Systematic debugging workflow
- Common issues and solutions
- Debugging techniques
- Tool usage (logs, Swagger, MongoDB)

**Example usage**:
```
"Following the debug-issues prompt, help me fix the null reference error"
```

## 🎭 Chat Modes

Chat modes set specialized behavior for Copilot interactions.

### [chatmodes/development.chatmode.md](chatmodes/development.chatmode.md)
**Use when**: Actively developing features

Optimizes Copilot for:
- Clean Architecture adherence
- Following project conventions
- Implementing features systematically
- Writing tests alongside code
- Maintaining quality standards

**Activation**: This mode is applied when actively working on code.

## 💡 How to Use These Files

### In VS Code Chat

**Reference a prompt**:
```
@workspace /help I want to add a new endpoint, use the add-endpoint prompt
```

**Reference instructions**:
```
@workspace Following the C# instructions, help me refactor this service
```

**Ask about guidelines**:
```
@workspace What are the testing requirements from testing.instructions.md?
```

### In Code Comments

Copilot automatically considers:
- `copilot-instructions.md` (always)
- Instruction files matching your current file pattern
- Context from open files

### During Code Review

Reference these files to ensure:
- Code follows project standards
- Tests are comprehensive
- Documentation is complete
- Architecture principles are maintained

## 🎯 Common Workflows

### Adding a New Feature
1. Read `copilot-instructions.md` for context
2. Use `prompts/add-endpoint.prompt.md` as a guide
3. Follow `instructions/csharp.instructions.md` for code style
4. Use `instructions/testing.instructions.md` for tests
5. Reference `instructions/api-controllers.instructions.md` for controllers

### Debugging an Issue
1. Use `prompts/debug-issues.prompt.md` for systematic approach
2. Check `copilot-instructions.md` for common patterns
3. Review relevant instruction files for code standards

### Writing Tests
1. Use `prompts/add-tests.prompt.md` as a template
2. Follow `instructions/testing.instructions.md` for patterns
3. Reference `copilot-instructions.md` for project structure

### Code Review
1. Check against `copilot-instructions.md` standards
2. Verify C# code follows `instructions/csharp.instructions.md`
3. Ensure tests follow `instructions/testing.instructions.md`
4. Confirm API follows `instructions/api-controllers.instructions.md`

## 📚 Additional Documentation

### Project Documentation
- `README.md` - Project overview, setup, features
- `NEWS_API_DOCUMENTATION.md` - Complete API reference
- `SWAGGER_TESTING_GUIDE.md` - Interactive testing guide
- `CONTRIBUTING.md` - Contribution guidelines

### Architecture Documentation
- `specs/002-modernize-net-core/spec.md` - Architecture specifications
- `specs/002-modernize-net-core/plan.md` - Implementation plan

### Test Documentation
- `NewsApi.Tests/TEST_COVERAGE_REPORT.md` - Test coverage details

## 🔄 Keeping Documentation Updated

These files are living documents that evolve with the project:

- Update `copilot-instructions.md` when project structure changes
- Update instruction files when coding patterns change
- Update prompts when workflows improve
- Create new instruction files for new contexts
- Create new prompts for common new tasks

## ❓ Getting Help

**Can't find what you need?**
1. Check the main `copilot-instructions.md`
2. Search across all instruction files
3. Review the prompt files for similar tasks
4. Ask Copilot: "@workspace where is the documentation for [topic]?"

**Found an issue with documentation?**
1. Open an issue on GitHub
2. Submit a PR with improvements
3. Follow the CONTRIBUTING.md guidelines

## 🎓 Best Practices

1. **Always reference copilot-instructions.md** - It's your foundation
2. **Use prompts as templates** - They provide proven workflows
3. **Follow instruction files strictly** - They ensure consistency
4. **Update docs when making changes** - Keep them accurate
5. **Share improvements** - Contribute better patterns back

## 🚀 Quick Reference

| Task | Primary File | Supporting Files |
|------|-------------|------------------|
| Add Endpoint | add-endpoint.prompt.md | csharp.instructions.md, api-controllers.instructions.md |
| Write Tests | add-tests.prompt.md | testing.instructions.md |
| Debug Issue | debug-issues.prompt.md | copilot-instructions.md |
| Review Code | copilot-instructions.md | All instruction files |
| Understand Architecture | copilot-instructions.md | specs/002-modernize-net-core/spec.md |

---

**Last Updated**: October 2025  
**Maintained By**: News API Team

**Questions?** Reference these docs or ask in GitHub Discussions.
