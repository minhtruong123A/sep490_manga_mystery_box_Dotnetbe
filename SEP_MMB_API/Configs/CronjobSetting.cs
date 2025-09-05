using Services.Cron;

namespace SEP_MMB_API.Configs;

public static class CronjobSetting
{
    public static IServiceCollection AddCronJobs(this IServiceCollection services)
    {
        services.AddHostedService<ImageWarmupCron>();
        return services;
    }
}