using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsApi.Presentation.Extensions;
using NewsApi.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "News API",
            Version = "v1",
            Description =
                "A comprehensive news management API with JWT authentication and flexible content management capabilities.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "News API Support",
                Email = "support@newsapi.com",
            },
        }
    );

    // Enable XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add custom application services
builder.Services.AddApplicationServices(builder.Configuration);

// Add response caching for optimization (30 minutes to minimize backend load)
builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(30)));
});

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS policy
builder.Services.AddCorsPolicy();

// Add health checks
builder.Services.AddCustomHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "News API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "News API Documentation";
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(1);
    });
}

// Security middleware (before routing)
app.UseMiddleware<SecurityHeadersMiddleware>();

// CORS (before authentication)
app.UseCors("AllowSpecificOrigins");

// Response caching
app.UseResponseCaching();
app.UseOutputCache();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom validation middleware
app.UseMiddleware<ValidationMiddleware>();

// Routing
app.MapControllers();

// Health checks
app.MapHealthChecks("/health");

await app.RunAsync().ConfigureAwait(false);
