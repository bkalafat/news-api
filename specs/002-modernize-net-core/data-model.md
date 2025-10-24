# Data Model: Clean Architecture Entities

## Domain Entities

### News Entity
```csharp
namespace NewsApi.Domain.Entities
{
    public class News
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string SocialTags { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImgPath { get; set; } = string.Empty;
        public string ImgAlt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string[] Subjects { get; set; } = Array.Empty<string>();
        public string[] Authors { get; set; } = Array.Empty<string>();
        public DateTime ExpressDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public string Url { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public bool IsSecondPageNews { get; set; }
    }
}
```

### Repository Interface
```csharp
namespace NewsApi.Domain.Interfaces
{
    public interface INewsRepository
    {
        Task<List<News>> GetAllAsync();
        Task<News?> GetByIdAsync(Guid id);
        Task<News?> GetByUrlAsync(string url);
        Task<News> CreateAsync(News news);
        Task UpdateAsync(Guid id, News news);
        Task DeleteAsync(Guid id);
    }
}
```

## Application Layer

### Service Interface
```csharp
namespace NewsApi.Application.Services
{
    public interface INewsService
    {
        Task<List<News>> GetAllAsync();
        Task<News?> GetByIdAsync(Guid id);
        Task<News?> GetByUrlAsync(string url);
        Task<News> CreateAsync(News news);
        Task UpdateAsync(Guid id, News news);
        Task DeleteAsync(Guid id);
    }
}
```

### DTOs
```csharp
namespace NewsApi.Application.DTOs
{
    public record CreateNewsDto(
        string Category,
        string Type,
        string Caption,
        string Summary,
        string Content,
        string[] Subjects,
        string[] Authors,
        DateTime ExpressDate,
        int Priority,
        bool IsActive,
        string Url
    );

    public record UpdateNewsDto(
        string? Category,
        string? Type,  
        string? Caption,
        string? Summary,
        string? Content,
        string[]? Subjects,
        string[]? Authors,
        DateTime? ExpressDate,
        int? Priority,
        bool? IsActive,
        string? Url
    );
}
```

### Validation Rules
```csharp
namespace NewsApi.Application.Validators
{
    public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
    {
        public CreateNewsDtoValidator()
        {
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Caption).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Summary).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.Subjects).NotEmpty();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.ExpressDate).GreaterThan(DateTime.MinValue);
            RuleFor(x => x.Priority).GreaterThan(0);
            RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        }
    }
}
```

## Infrastructure Layer

### Configuration Settings
```csharp
namespace NewsApi.Infrastructure.Data.Configurations
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string NewsCollectionName { get; set; } = string.Empty;
    }
}
```

### MongoDB Context
```csharp
namespace NewsApi.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<News> News => 
            _database.GetCollection<News>("News");
    }
}
```

## Relationships and Constraints

- **News Entity**: Primary aggregate root with no external dependencies
- **Repository Pattern**: Abstract data access through interfaces
- **Service Layer**: Business logic coordination between repositories
- **Validation**: FluentValidation rules enforce business constraints
- **Caching**: Memory cache integration for performance optimization
- **Authentication**: JWT Bearer token validation for protected endpoints

## Migration Strategy

1. **Schema Compatibility**: New entity structure maintains backward compatibility with existing MongoDB documents
2. **Data Migration**: Scripts to transform existing data to new schema format
3. **Index Preservation**: Maintain existing MongoDB indexes for performance
4. **Validation Migration**: Convert existing basic validation to FluentValidation rules