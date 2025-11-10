namespace OutputCachingBasics.Extensions;

public static class OutputCacheExtension
{
    public static void AddOutputCachingBasics(this IServiceCollection services)
    {
        services.AddOutputCache( options => 
        {
            
           options.AddBasePolicy(o => o.Expire(TimeSpan.FromMinutes(5)));

           options.AddPolicy("AuthenticatedUserCachePolicy", policy =>
           {
               policy.SetVaryByHeader("Authorization").Expire(TimeSpan.FromMinutes(1));
           });
           
           options.AddPolicy("CacheForOneMinute", policy =>
           {
               policy.Expire(TimeSpan.FromMinutes(1));
           });
           
           options.AddPolicy("NoCache", policy =>
           {
               policy.NoCache();
           });


        });
    }
}