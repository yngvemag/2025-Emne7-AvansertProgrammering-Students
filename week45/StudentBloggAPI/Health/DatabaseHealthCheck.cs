using Microsoft.Extensions.Diagnostics.HealthChecks;
using StudentBloggAPI.Data;

namespace StudentBloggAPI.Health;

public class DatabaseHealthCheck(StudentBloggDbContext dbContext): IHealthCheck
{
    private readonly StudentBloggDbContext _dbContext = dbContext;
        
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = new CancellationToken())
    {
        try
        {
            if (await _dbContext.Database.CanConnectAsync(ct))
            {
                return  HealthCheckResult.Healthy("Database connection is healthy");
            }
            else
            {
                return  HealthCheckResult.Unhealthy("Database connection is NOT healthy");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed.");
        }
    }
}