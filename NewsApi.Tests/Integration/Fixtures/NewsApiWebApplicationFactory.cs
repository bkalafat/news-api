using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using NewsApi.Infrastructure.Data;
using NewsApi.Infrastructure.Data.Configurations;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Integration.Fixtures;

public class NewsApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public string TestDatabaseName { get; } = $"NewsApiTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ensure environment variables are set so application code that uses
        // Environment.GetEnvironmentVariable can detect the testing environment.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        // Configure to use Testing environment and load Testing settings
        builder
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                // The hostContext should have ContentRootPath set to the newsApi project directory
                // Load the base appsettings.json first
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                // Then overlay the Testing-specific settings
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", 
                    optional: true, reloadOnChange: false);

                // Override the MongoDB database name so the application uses the generated test DB from startup
                var overrides = new Dictionary<string, string>
                {
                    { "MongoDbSettings:DatabaseName", TestDatabaseName }
                };
                config.AddInMemoryCollection(overrides as IEnumerable<KeyValuePair<string, string?>>);

                // Attempt to drop any leftover test databases early so the app starts with a clean DB.
                try
                {
                    var built = config.Build();
                    var conn = built["MongoDbSettings:ConnectionString"] ?? "mongodb://localhost:27017";
                    var client = new MongoClient(conn);
                    client.DropDatabase(TestDatabaseName);
                    try { client.DropDatabase("NewsApiTestDb"); } catch { }
                }
                catch
                {
                    // ignore - this is a best-effort cleanup before the host starts
                }
            });

        builder.ConfigureTestServices(services =>
        {
            // Replace IMemoryCache with NullMemoryCache for integration tests
            // This prevents cross-test contamination via caching
            services.RemoveAll<IMemoryCache>();
            services.AddSingleton<IMemoryCache, NullMemoryCache>();

            // Remove existing MongoDB registration
            services.RemoveAll<MongoDbContext>();
            services.RemoveAll<IMongoDatabase>();
            services.RemoveAll<MongoDbSettings>();

            // Add test MongoDB configuration
            var testSettings = new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = TestDatabaseName
            };

            services.AddSingleton(testSettings);
            services.AddSingleton<MongoDbContext>();

            // Clean up test database(s) - drop both the generated DB and the static testing DB
            try
            {
                var client = new MongoClient(testSettings.ConnectionString);
                client.DropDatabase(TestDatabaseName);
                // Also drop the static testing DB name from appsettings.Testing.json in case it was used
                try
                {
                    client.DropDatabase("NewsApiTestDb");
                }
                catch
                {
                    // ignore
                }
            }
            catch
            {
                // Ignore errors - database might not exist yet or Mongo not reachable
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clean up test database
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                client.DropDatabase(TestDatabaseName);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        base.Dispose(disposing);
    }

    // Hide the base CreateClient so we can ensure the database is cleaned before each test uses the factory.
    // Tests call factory.CreateClient() in their constructors; this "new" method will be invoked and
    // perform a best-effort DropDatabase to guarantee per-test isolation.
    public new System.Net.Http.HttpClient CreateClient()
    {
        try
        {
            var client = new MongoClient("mongodb://localhost:27017");
            client.DropDatabase(TestDatabaseName);
            try { client.DropDatabase("NewsApiTestDb"); } catch { }
        }
        catch
        {
            // ignore
        }

        return base.CreateClient();
    }
}
