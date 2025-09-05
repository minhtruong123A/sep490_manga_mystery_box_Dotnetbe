using AspNetCoreRateLimit;

namespace SEP_MMB_API.Configs;

public static class RateLimiterSetting
{
    public static IServiceCollection AddRateLimiter(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        return services;
    }
}