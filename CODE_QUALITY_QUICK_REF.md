# Code Quality Quick Reference

## 🚀 Quick Commands

```powershell
# Run full analysis
.\analyze-code.ps1

# Auto-fix formatting
.\analyze-code.ps1 -Fix

# Strict mode (fail on warnings)
.\analyze-code.ps1 -FailOnWarnings

# Format code only
dotnet format

# Build with analysis
dotnet build /p:EnforceCodeStyleInBuild=true
```

## 🔍 Common Issues & Fixes

### Unused Code

```csharp
// ❌ Problem: Unused variable
int unusedVar = 10;

// ✅ Solution: Remove it
// (Just delete the line)

// ❌ Problem: Unused parameter
public void Method(int unused)

// ✅ Solution: Remove or use discard
public void Method(int _)  // or remove parameter

// ❌ Problem: Unused private method
private void UnusedMethod() { }

// ✅ Solution: Delete the method
```

### Redundant Code

```csharp
// ❌ Problem: Redundant assignment
int x = 5;
x = 10;  // First assignment is redundant

// ✅ Solution: Remove first assignment
int x = 10;

// ❌ Problem: Redundant ToString()
string s = number.ToString();
Console.WriteLine(s.ToString());

// ✅ Solution: Remove redundant call
Console.WriteLine(s);

// ❌ Problem: Redundant async/await
public async Task<int> GetValue()
{
    return await SomeAsyncMethod();
}

// ✅ Solution: Remove async/await
public Task<int> GetValue()
{
    return SomeAsyncMethod();
}
```

### Performance Issues

```csharp
// ❌ Problem: Using Count() when Any() is faster
if (items.Count() > 0)

// ✅ Solution: Use Any()
if (items.Any())

// ❌ Problem: Using Enumerable.Count()
if (list.Count() > 5)

// ✅ Solution: Use Length/Count property
if (list.Count > 5)

// ❌ Problem: String.Contains with string
if (text.Contains("a"))

// ✅ Solution: Use char for single character
if (text.Contains('a'))
```

### Code Style

```csharp
// ❌ Problem: Block-scoped namespace
namespace NewsPortal
{
    public class MyClass { }
}

// ✅ Solution: File-scoped namespace
namespace NewsPortal;

public class MyClass { }

// ❌ Problem: Using inside namespace
namespace NewsPortal
{
    using System;
}

// ✅ Solution: Using outside namespace
using System;

namespace NewsPortal;

// ❌ Problem: Missing braces
if (condition)
    DoSomething();

// ✅ Solution: Always use braces
if (condition)
{
    DoSomething();
}
```

## 🎯 Most Important Rules

| Rule | Description | Fix |
|------|-------------|-----|
| **IDE0051** | Remove unused private member | Delete the member |
| **IDE0052** | Remove unread private member | Delete or use it |
| **IDE0059** | Unnecessary assignment | Remove redundant assignment |
| **IDE0060** | Remove unused parameter | Delete parameter or use `_` |
| **RCS1213** | Remove unused member | Delete the member |
| **RCS1174** | Remove redundant async/await | Return Task directly |
| **CA1827** | Use Any() instead of Count() | Replace with `.Any()` |
| **CA1829** | Use Count property | Use `.Count` not `.Count()` |

## 🛠️ IDE Quick Fixes

### Visual Studio
- **Ctrl+.** → Quick Actions menu
- **Ctrl+K, Ctrl+D** → Format document
- **Ctrl+E, C** → Comment selection

### Rider
- **Alt+Enter** → Quick Actions menu
- **Ctrl+Alt+L** → Format code
- **Ctrl+/** → Comment/uncomment

### VS Code
- **Ctrl+.** → Quick Actions menu
- **Shift+Alt+F** → Format document
- **Ctrl+/** → Comment/uncomment

## 📊 Rule Severity Levels

```ini
# In .editorconfig:

dotnet_diagnostic.RULEID.severity = error      # ❌ Build fails
dotnet_diagnostic.RULEID.severity = warning    # ⚠️ Show warning
dotnet_diagnostic.RULEID.severity = suggestion # 💡 IDE hint
dotnet_diagnostic.RULEID.severity = none       # 🔇 Disabled
```

## 🔕 Suppress Warnings

### In Code (Temporary)
```csharp
#pragma warning disable RCS1163
public void Method(int param)
{
    // param intentionally unused
}
#pragma warning restore RCS1163
```

### In .editorconfig (Permanent)
```ini
[*.cs]
dotnet_diagnostic.RCS1163.severity = none
```

### With Attribute (Specific)
```csharp
[SuppressMessage("Usage", "CA1801", Justification = "Used by framework")]
public void Method(int param)
{
}
```

## 📈 Typical Analysis Workflow

1. **Run analysis**: `.\analyze-code.ps1`
2. **Review warnings**: Check console output
3. **Open in IDE**: See inline suggestions
4. **Fix issues**: Use Quick Actions (Ctrl+.)
5. **Auto-format**: `dotnet format`
6. **Re-run analysis**: Verify fixes
7. **Commit**: Push clean code

## 🎓 Learning Resources

- [Roslynator Rules](https://josefpihrt.github.io/docs/roslynator/analyzers/)
- [CA Rules](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/)
- [IDE Rules](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/)

## 🆘 Need Help?

```powershell
# Verbose analysis with full details
.\analyze-code.ps1 -Verbose

# Check specific project
dotnet build backend\newsApi.csproj --no-restore

# View all warnings
dotnet build | Select-String "warning"

# Count warnings
(dotnet build 2>&1 | Select-String "warning").Count
```

## 📝 Tips

1. **Start small**: Fix one category at a time
2. **Use Quick Actions**: Let IDE suggest fixes
3. **Review suggestions**: Learn better patterns
4. **Run before commit**: Catch issues early
5. **Don't ignore warnings**: They indicate real issues
6. **Update regularly**: Keep analyzers current

---

**Pro Tip**: Set up a pre-commit hook to run `dotnet format` automatically!
