using BusinessObjects.Mongodb;
using MongoDB.Driver;

namespace SEP_MMB_API.Configs;

public static class DatabaseSetting
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IMongoClient>(s =>
            new MongoClient(configuration.GetConnectionString("MongoDb"))
        );
        return services;
    }
}