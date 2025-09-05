using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Options;
using Net.payOS;
using Services.Helper.Supabase;
using Services.Interface;

namespace SEP_MMB_API.Configs;

public static class ConfigLoaderSetting
{
    public static IServiceCollection AddConfigLoader(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<PayOSConfig>(configuration.GetSection("PayOS"));
        var config = configuration.GetSection("PayOS").Get<PayOSConfig>();
        services.AddSingleton(new PayOS(config.ClientId, config.ApiKey, config.ChecksumKey));
        services.Configure<SupabaseSettings>(
            configuration.GetSection("Supabase")
        );
        services.AddSingleton<ISupabaseStorageHelper, SupabaseStorageHelper>();
        //Feesettings
        services.Configure<FeeSettings>(configuration.GetSection("FeeSettings"));
        //Favoritesettings
        services.Configure<FavoritesSettings>(configuration.GetSection("FavoritesSettings"));
        //Rewardsettings
        services.Configure<RewardSettings>(configuration.GetSection("RewardSettings"));
        //Exchangesettings
        services.Configure<ExchangeSettings>(configuration.GetSection("ExchangeSettings"));
        //Productpricesettings
        services.Configure<ProductPriceSettings>(configuration.GetSection("ProductPriceSettings"));
        //Withdrawrulessettings
        services.Configure<WithdrawRulesSettings>(configuration.GetSection("WithdrawRules"));
        //Imageproxysettings
        services.Configure<ImageProxySettings>(configuration.GetSection("ImageProxySettings"));
        return services;
    }
}