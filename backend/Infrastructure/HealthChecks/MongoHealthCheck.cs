using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NewsApi.Infrastructure.Data;

namespace NewsApi.Infrastructure.HealthChecks;

public class MongoHealthCheck : IHealthCheck
{
    private readonly MongoDbContext _context;

    public MongoHealthCheck(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // Test database connectivity by running a simple command
            var database = _context.News.Database;
            await database
                .RunCommandAsync<object>("{ ping: 1 }", cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            // Optionally check collection accessibility
            var collectionCount = await _context
                .News.EstimatedDocumentCountAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckResult.Healthy(
                $"MongoDB is healthy. Collection has approximately {collectionCount} documents."
            );
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"MongoDB health check failed: {ex.Message}", ex);
        }
    }
}
