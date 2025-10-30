# News API Platform - Cursor AI Rules

## Project Overview

Full-stack Turkish tech news platform:
- **Backend**: .NET 9 Web API with Clean Architecture, MongoDB, JWT auth
- **Frontend**: Next.js 16 with TypeScript, TailwindCSS v4, React Query, Turkish i18n

## General Rules

1. **Always use Context7 MCP** for up-to-date documentation
   - Next.js 16, React Query, TailwindCSS v4, .NET 9, MongoDB
   - Add "use context7" to prompts automatically when working with these technologies

2. **Follow project architecture**
   - Backend: Clean Architecture (Domain → Application → Infrastructure → Presentation)
   - Frontend: Feature-based component organization

3. **TypeScript everywhere**
   - Strict type checking enabled
   - No `any` types without justification
   - Use proper interfaces and types

4. **Code style**
   - Backend: C# conventions, async/await, dependency injection
   - Frontend: React hooks, composition over inheritance, functional components

## Backend (.NET) Rules

### Architecture
- Keep Domain layer pure (no external dependencies)
- Application layer for business logic
- Infrastructure for external concerns
- Presentation for HTTP/API concerns

### Coding Standards
- Use async/await for all I/O operations
- Never use `.Result` or `.Wait()` on async methods
- Constructor injection for dependencies
- Repository pattern for data access
- FluentValidation for input validation

### API Controllers
- Return `ActionResult<T>` from controller methods
- Use `[ApiController]` attribute
- Add `[ProducesResponseType]` for all responses
- Keep controllers thin - delegate to services

### MongoDB
- Use official MongoDB.Driver
- Async operations only
- Proper error handling with try-catch

## Frontend (Next.js) Rules

### Next.js 16 Patterns
- Use App Router (not Pages Router)
- Server Components by default, 'use client' when needed
- TypeScript for all files
- Proper metadata API for SEO

### React Query
- Use custom hooks from `lib/api/hooks.ts`
- Proper query keys for cache management
- Optimistic updates where appropriate
- Error handling with retry logic

### Components
- Functional components with TypeScript
- Use Shadcn/ui components for consistency
- TailwindCSS for styling
- No inline styles unless absolutely necessary

### Styling
- TailwindCSS v4 utilities
- Use `cn()` utility for conditional classes
- Mobile-first responsive design
- Semantic HTML elements

### State Management
- React Query for server state
- React hooks for local state
- No prop drilling - use composition

### i18n
- Use `useTranslations()` hook from next-intl
- All user-facing text must be translated
- Translation keys in `messages/tr.json`

## Documentation Rules

### Code Comments
- XML comments for public APIs (C#)
- JSDoc for exported functions (TypeScript)
- Explain "why", not "what"

### README Files
- Keep updated with architecture changes
- Include setup instructions
- Add troubleshooting section

## Testing Rules

### Backend
- Unit tests for services and validators
- Integration tests for controllers and repositories
- AAA pattern (Arrange-Act-Assert)
- Mock external dependencies with Moq

### Frontend
- (To be implemented)
- Test user interactions
- Mock API calls
- Test edge cases

## Security Rules

### Backend
- Never hardcode secrets
- Use User Secrets (dev) or Azure Key Vault (prod)
- Validate all user input
- Implement proper CORS
- Security headers middleware

### Frontend
- Sanitize user input
- Use environment variables for config
- Implement CSP headers
- HTTPS only in production

## Performance Rules

### Backend
- Enable memory caching where appropriate
- Use projections in MongoDB queries
- Async I/O operations
- Connection pooling

### Frontend
- Use Next.js Image component
- Lazy load components and images
- Code splitting
- React Query caching

## Git Workflow

- Feature branches from `main`
- Clear, descriptive commit messages
- PR reviews required
- Keep commits atomic

## Context7 Usage

Always use Context7 for:
- Next.js 16 features (App Router, Metadata API, Server Actions)
- React Query patterns (useQuery, useMutation, cache management)
- TailwindCSS v4 utilities and configuration
- Shadcn/ui component usage
- .NET 9 features (C# 12, ASP.NET Core, minimal APIs)
- MongoDB .NET Driver methods
- FluentValidation rules
- Authentication patterns (JWT)

## Common Patterns

### Backend - Adding New Endpoint
1. Create DTO in `Application/DTOs/`
2. Add validator in `Application/Validators/`
3. Implement service method in `Application/Services/`
4. Add controller action in `Presentation/Controllers/`
5. Write tests

### Frontend - Adding New Page
1. Create page in `app/[route]/page.tsx`
2. Add translations in `messages/tr.json`
3. Create components in `components/[feature]/`
4. Add API hook if needed in `lib/api/hooks.ts`
5. Update metadata for SEO

### Frontend - Adding New Component
1. Create component file in appropriate folder
2. Use TypeScript with proper interfaces
3. Use Shadcn/ui components when possible
4. Add responsive design with TailwindCSS
5. Export from index if part of feature group

## Error Handling

### Backend
```csharp
try
{
    var result = await _service.GetDataAsync();
    return Ok(result);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred");
    return StatusCode(500, "Internal server error");
}
```

### Frontend
```typescript
const { data, error, isLoading } = useQuery({
    queryKey: ['news'],
    queryFn: fetchNews,
    retry: 3,
    onError: (error) => {
        console.error('Failed to fetch news:', error);
    }
});
```

## Quick References

### Backend Namespaces
- `newsApi.Domain` - Pure business entities
- `newsApi.Application` - Business logic & DTOs
- `newsApi.Infrastructure` - External concerns
- `newsApi.Presentation` - Controllers & middleware

### Frontend Paths
- `@/components` - React components
- `@/lib` - Utilities, API client, hooks
- `@/app` - Next.js pages (App Router)

---

**Remember**: When in doubt about any library or API usage, use Context7 MCP to fetch the most current documentation!

