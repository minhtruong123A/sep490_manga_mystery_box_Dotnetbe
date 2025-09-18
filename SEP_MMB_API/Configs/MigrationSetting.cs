using BusinessObjects.Mongodb;

namespace SEP_MMB_API.Configs;

public static class MigrationSetting
{
    public static async Task<IApplicationBuilder> UseMongoSeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        var seeder = new MongoSeeder(dbContext);
        await seeder.SeedAsync();
        return app;
    }
}