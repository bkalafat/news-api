# 🎯 Frontend Code Quality - Quick Reference

## One-Command Cleanup

```bash
cd frontend
npm run code-quality:fix
```

This runs: TypeScript check → ESLint (auto-fix) → Prettier (format)

## Daily Commands

| Command                    | What It Does          |
| -------------------------- | --------------------- |
| `npm run lint`             | Check for code issues |
| `npm run lint:fix`         | Auto-fix issues       |
| `npm run format`           | Format all files      |
| `npm run type-check`       | TypeScript validation |
| `npm run code-quality:fix` | Run all + auto-fix    |

## What Was Removed Today

✅ **12 unused imports** automatically deleted:

- `Badge` (3 files)
- `Label`, `Users`, `Clock` (admin files)
- `Share2`, `BookmarkPlus` (news components)
- `vi` (test file)
- `authFetch`, `CardContent`, `Button`, `useTranslations`

✅ **1 code style issue** fixed:

- String concatenation converted to template literals

## Remaining Items (Non-Critical)

⚠️ **6 warnings** need manual review:

- 3 unused variables (assigned but not used)
- 2 console.log statements (use console.warn/error instead)
- 1 lonely-if (can be simplified)

## VS Code Setup (30 seconds)

1. Open project in VS Code
2. Click "Install" on the extensions popup
3. Reload VS Code
4. **Done!** Code now auto-formats and auto-fixes on save

## Rules Enabled

### Unused Code Detection

- ✅ Unused imports → Automatically removed
- ✅ Unused variables → Warning
- ✅ Unused functions → Warning
- ✅ Unused parameters → Warning (prefix with `_` if intentional)

### Code Style

- ✅ No `var`, use `const`/`let`
- ✅ Template literals over string concatenation
- ✅ Arrow functions preferred
- ✅ Object shorthand
- ✅ No redundant else-return

### TypeScript

- ✅ Strict mode enabled
- ✅ No unused locals/parameters
- ✅ All code paths must return
- ✅ No implicit any

### Formatting

- ✅ 2-space indentation
- ✅ Double quotes
- ✅ Semicolons
- ✅ 100-char line length
- ✅ Unix line endings (LF)

## Files Created

```
frontend/
├── eslint.config.mjs          # ESLint rules
├── .prettierrc.json           # Prettier config
├── .prettierignore            # Prettier ignore
├── .editorconfig              # Editor settings
├── CODE_QUALITY.md            # Full documentation
├── SETUP_COMPLETE.md          # Setup summary
└── .vscode/
    ├── settings.json          # VS Code auto-format
    └── extensions.json        # Recommended extensions
```

## GitHub Actions

✅ Automatic checks on every push:

- TypeScript type check
- ESLint validation
- Prettier format check
- Unit tests
- Build verification

See: `.github/workflows/frontend-code-quality.yml`

## Example: Fix Unused Variable

```typescript
// ❌ Before (warning)
const handleShare = () => { /* ... */ };

// ✅ After (no warning) - Either use it:
<button onClick={handleShare}>Share</button>

// ✅ Or prefix with _ to mark intentionally unused:
const _handleShare = () => { /* ... */ };
```

## Example: Console Statements

```typescript
// ❌ Bad
console.log("Debug info");

// ✅ Good
console.warn("Warning message");
console.error("Error message");

// ✅ Or remove in production:
if (process.env.NODE_ENV === "development") {
  console.log("Debug info");
}
```

## Get Help

- **Full Docs**: `CODE_QUALITY.md`
- **ESLint Rules**: https://eslint.org/docs/rules/
- **Prettier Options**: https://prettier.io/docs/en/options.html
- **TypeScript Config**: https://www.typescriptlang.org/tsconfig

---

**Status**: ✅ Active  
**Issues Found**: 6 warnings (non-blocking)  
**Auto-Fixed**: 12 unused imports + 1 style issue  
**Next Step**: Fix remaining 6 warnings manually
