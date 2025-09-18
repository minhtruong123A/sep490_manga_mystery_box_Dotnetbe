using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interface;

namespace Services.Cron;

public class ImageWarmupCron(ILogger<ImageWarmupCron> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    : BackgroundService
{
    private readonly CronExpression _expression = CronExpression.Parse(configuration["CronJobs:MyJob"] ?? "*/5 * * * *");
    private readonly TimeZoneInfo _timeZoneInfo = TimeZoneInfo.Local;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (!next.HasValue) continue;

            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds > 0)
                await Task.Delay(delay, stoppingToken);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                await imageService.WarmUpImageCacheAsync();
                logger.LogInformation("Image cache warmed up at {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while warming up image cache");
            }
        }
    }
}