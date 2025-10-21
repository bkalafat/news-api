using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using NewsApi.Infrastructure.Data;
using NewsApi.Infrastructure.Data.Configurations;

namespace NewsApi.Tests.Integration.Fixtures;

public class NewsApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public string TestDatabaseName { get; } = $"NewsApiTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
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

          // Build service provider to initialize database
       var serviceProvider = services.BuildServiceProvider();
          var context = serviceProvider.GetRequiredService<MongoDbContext>();
            
      // Ensure database is clean
    var client = new MongoClient(testSettings.ConnectionString);
      client.DropDatabase(TestDatabaseName);
        });

        builder.UseEnvironment("Testing");
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
}
