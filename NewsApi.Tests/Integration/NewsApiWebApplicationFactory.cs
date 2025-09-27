using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsApi.Infrastructure.Data.Configurations;
using Testcontainers.MongoDb;

namespace NewsApi.Tests.Integration;

public class NewsApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;

    public NewsApiWebApplicationFactory()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .WithPortBinding(27017, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDbSettings:ConnectionString"] = _mongoContainer.GetConnectionString(),
                ["MongoDbSettings:DatabaseName"] = "TestNewsDb",
                ["MongoDbSettings:NewsCollectionName"] = "News"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing MongoDbSettings registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MongoDbSettings));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test-specific MongoDbSettings
            services.AddSingleton(new MongoDbSettings
            {
                ConnectionString = _mongoContainer.GetConnectionString(),
                DatabaseName = "TestNewsDb",
                NewsCollectionName = "News"
            });
        });

        builder.UseEnvironment("Testing");
    }
}