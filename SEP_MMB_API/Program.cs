using AspNetCoreRateLimit;
using SEP_MMB_API.Configs;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var devPassword = builder.Configuration["DevSettings:DevPassword"];
builder.Services.AddConfigLoader(configuration)
    .AddSwaggerConfiguration()
    .AddSecurityConfiguration(configuration)
    .AddRateLimiter(configuration)
    .AddDatabaseConfiguration(configuration)
    .AddRepositories()
    .AddServices()
    .AddCronJobs();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseIpRateLimiting();

app.UseAuthentication();

app.UseDevPasswordProtection(devPassword!)
    .UseMongoInjectionFilter()
    .UseRefreshTokenRestriction().UseSwaggerConfiguration()
    .UseForwardConfig()
    .UseSwaggerConfiguration();
app.UseAuthorization();
app.MapControllers();

// await app.UseMongoSeedAsync();

app.Run();
