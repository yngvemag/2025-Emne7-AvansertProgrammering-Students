using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimitingBasics.Extensions;

public static class RateLimitServiceExtension
{
    public static void AddRateLimitingBasics(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            // fixed window
            options.AddFixedWindowLimiter("fixed", o =>
            {
                o.PermitLimit = 2;
                o.Window = TimeSpan.FromSeconds(10);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 1;
            });
            
            // sliding window
            options.AddSlidingWindowLimiter("sliding", o =>
            {
                o.PermitLimit = 2;
                o.Window = TimeSpan.FromSeconds(10);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 1;
                o.SegmentsPerWindow = 2;
            });
            
            // token buckets
            options.AddTokenBucketLimiter("token_bucket", o =>
            {
                o.TokenLimit = 10;
                o.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                o.TokensPerPeriod = 10;
            });
            
            // concurrency
            options.AddConcurrencyLimiter("concurrency", o =>
            {
                o.PermitLimit = 3;
            });
        });
    }
}