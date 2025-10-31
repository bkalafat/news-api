# Code Quality Configuration

## 🎯 Overview

This repository uses a comprehensive set of code analyzers and rules to maintain high code quality, find redundant code, unused variables, and enforce best practices.

## 📦 Installed Analyzers

### 1. **Roslynator.Analyzers** (v4.14.1)
- 500+ code analysis rules
- Finds redundant code, unused variables, and optimization opportunities
- Suggests modern C# patterns and best practices

### 2. **StyleCop.Analyzers** (v1.2.0-beta.556)
- Enforces consistent code style and documentation
- Naming conventions, ordering, and layout rules
- Configured in `backend/stylecop.json`

### 3. **Meziantou.Analyzer** (v2.0.177)
- Performance-focused analyzer
- Detects async/await issues, string handling, and memory allocations
- API best practices for ASP.NET Core

### 4. **SonarAnalyzer.CSharp** (v10.4.0)
- Code smell detection
- Security vulnerability scanning
- Maintainability and reliability checks

### 5. **Microsoft.CodeAnalysis.NetAnalyzers** (v9.0.0)
- Official .NET code analysis
- Performance, security, and design rules
- Latest C# 12+ best practices

## 📄 Configuration Files

| File | Purpose | Scope |
|------|---------|-------|
| `.editorconfig` | Main code style and analyzer rules | All files |
| `.globalconfig` | Global analyzer configuration | All projects |
| `Directory.Build.props` | Shared build properties and analyzer packages | All projects |
| `backend/stylecop.json` | StyleCop-specific settings | Backend project |

## 🔍 Key Features

### Find Unused Code
The following analyzers detect unused code:
- **IDE0051**: Remove unused private member
- **IDE0052**: Remove unread private member
- **IDE0060**: Remove unused parameter
- **RCS1163**: Unused parameter
- **RCS1213**: Remove unused member declaration

### Find Redundant Code
- **RCS1074**: Remove redundant constructor
- **RCS1097**: Remove redundant ToString call
- **RCS1129**: Remove redundant field initialization
- **RCS1174**: Remove redundant async/await
- **RCS1188**: Remove redundant auto-property initialization
- **RCS1212**: Remove redundant assignment
- **IDE0004**: Remove unnecessary cast

### Performance Optimizations
- **CA1827**: Do not use Count() when Any() can be used
- **CA1829**: Use Length/Count property instead of Enumerable.Count
- **CA1846**: Prefer AsSpan over Substring
- **CA1847**: Use string.Contains(char) instead of Contains(string)

### Code Style Enforcement
- 4 spaces indentation
- File-scoped namespaces
- Using directives outside namespace
- Braces required for all control structures
- Private fields with underscore prefix (`_fieldName`)

## 🚀 Usage

### Quick Analysis
```powershell
# Run comprehensive code analysis
.\analyze-code.ps1

# Auto-fix formatting issues
.\analyze-code.ps1 -Fix

# Verbose output with details
.\analyze-code.ps1 -Verbose

# Strict mode (fail on warnings)
.\analyze-code.ps1 -FailOnWarnings
```

### Manual Commands
```powershell
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build with full analysis
dotnet build /p:EnforceCodeStyleInBuild=true

# Format code
dotnet format

# Verify formatting without changes
dotnet format --verify-no-changes
```

### IDE Integration

#### Visual Studio 2022
1. **Error List** → Shows all warnings and errors
2. **Solution Explorer** → Right-click project → **Analyze and Code Cleanup**
3. **Tools** → **Options** → **Text Editor** → **C#** → **Code Style**
4. Warnings appear as green squiggles in editor

#### Visual Studio Code
1. Install **C# Dev Kit** extension
2. Analyzers run automatically on save
3. View problems in **Problems** panel (Ctrl+Shift+M)

#### JetBrains Rider
1. **Code** → **Inspect Code**
2. **Code** → **Cleanup Code**
3. Settings → **Editor** → **Code Style** → **C#**
4. Warnings shown inline with quick-fixes (Alt+Enter)

## 📊 Analyzer Rule Severities

| Severity | Meaning | Action |
|----------|---------|--------|
| **Error** | Build fails | Must fix immediately |
| **Warning** | Should fix | Reported in build output |
| **Suggestion** | Consider fixing | Shown in IDE only |
| **None** | Disabled | No action |

## 🎯 Common Rules

### Must Fix (Warnings)
```csharp
// ❌ IDE0051: Unused private method
private void UnusedMethod() { }

// ❌ IDE0059: Unnecessary assignment
int x = 5;
x = 10;  // First assignment is redundant

// ❌ RCS1213: Unused member
private readonly string _unusedField;

// ❌ CA1849: Call async method in async context
public async Task DoWork()
{
    var result = GetData().Result;  // Use await instead
}
```

### Should Fix (Suggestions)
```csharp
// ⚠️ IDE0005: Remove unnecessary using
using System.Diagnostics;  // Not used in file

// ⚠️ RCS1118: Mark as const
var timeout = 30;  // Can be const

// ⚠️ RCS1170: Use read-only property
public int Count { get; set; }  // If never set externally
```

### Good Practices
```csharp
// ✅ File-scoped namespace
namespace NewsPortal.Application.Services;

// ✅ Private fields with underscore
private readonly INewsRepository _repository;

// ✅ Use pattern matching
if (value is not null)
{
    // ...
}

// ✅ String.Contains with char
if (text.Contains('a'))  // Instead of Contains("a")

// ✅ Use Length/Count property
if (items.Length > 0)  // Instead of items.Count() > 0
```

## 🔧 Customization

### Disable Specific Rule
In `.editorconfig`:
```ini
dotnet_diagnostic.RCS1163.severity = none  # Disable unused parameter warning
```

### Suppress in Code
```csharp
#pragma warning disable RCS1163 // Unused parameter
public void Method(int unusedParam)
{
    // ...
}
#pragma warning restore RCS1163
```

### Per-Project Configuration
Create `backend/.editorconfig` for project-specific overrides.

## 📈 Continuous Integration

The analyzers automatically run in CI/CD pipelines:
- GitHub Actions: Warnings reported in PR comments
- Azure DevOps: Build warnings tab
- Local builds: Console output with warnings

To treat warnings as errors in CI:
```xml
<PropertyGroup>
  <TreatWarningsAsErrors Condition="'$(CI)' == 'true'">true</TreatWarningsAsErrors>
</PropertyGroup>
```

## 🐛 Troubleshooting

### Too Many Warnings
1. Start with `severity = suggestion` for noisy rules
2. Gradually increase to `warning` as code improves
3. Disable rules that don't fit your team's style

### Performance Issues
1. Disable analyzers in `Directory.Build.props` for faster builds
2. Use `dotnet build --no-incremental` if analysis is cached incorrectly
3. Consider parallel build: `dotnet build -m`

### False Positives
1. Suppress specific instances with `#pragma warning disable`
2. Report issues to analyzer maintainers on GitHub
3. Disable problematic rules in `.editorconfig`

## 📚 Resources

- [Roslynator Documentation](https://josefpihrt.github.io/docs/roslynator/)
- [StyleCop Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Meziantou.Analyzer Rules](https://github.com/meziantou/Meziantou.Analyzer/tree/main/docs)
- [Microsoft Code Analysis](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

## 🎓 Best Practices

1. **Run analysis before committing**: `.\analyze-code.ps1`
2. **Fix warnings incrementally**: Don't try to fix everything at once
3. **Review analyzer suggestions**: They often teach better patterns
4. **Keep configuration in sync**: `.editorconfig` should match team standards
5. **Document suppressions**: Add comments explaining why rules are disabled

## 🔄 Maintenance

Update analyzers regularly:
```powershell
# Update all analyzer packages
dotnet list package --outdated --include-prerelease

# Update specific analyzer
dotnet add package Roslynator.Analyzers --version <latest>
```

## 📞 Support

For issues or questions:
1. Check analyzer documentation (links above)
2. Review `.editorconfig` and `.globalconfig` for rule configuration
3. Run `.\analyze-code.ps1 -Verbose` for detailed diagnostics
4. Open GitHub issue with specific warning codes

---

**Last Updated**: October 2025  
**Maintained By**: @bkalafat
