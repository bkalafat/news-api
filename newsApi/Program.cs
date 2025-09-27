using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsApi.Presentation.Extensions;
using NewsApi.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom application services
builder.Services.AddApplicationServices(builder.Configuration);

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
    app.UseSwaggerUI();
}

// Security middleware (before routing)
app.UseMiddleware<SecurityHeadersMiddleware>();

// CORS (before authentication)
app.UseCors("AllowSpecificOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom validation middleware
app.UseMiddleware<ValidationMiddleware>();

// Routing
app.MapControllers();

// Health checks
app.MapHealthChecks("/health");

app.Run();

// Make Program class accessible to tests
public partial class Program { }
