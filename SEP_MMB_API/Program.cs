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
app.UsePathBase("/cs");
app.Use((context, next) =>
{
    var prefix = context.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();
    if (!string.IsNullOrEmpty(prefix))
    {
        context.Request.PathBase = prefix;
    }
    return next();
});
app.UseRouting();
app.UseCors("AllowAll");
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.UseDevPasswordProtection(devPassword!)
    .UseMongoInjectionFilter()
    .UseRefreshTokenRestriction();

//allow /cs and / run together
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/cs", out var remaining))
//    {
//        context.Request.PathBase = "/cs";
//        context.Request.Path = remaining;
//    }

//    await next();
//});


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "Main User API");
    c.SwaggerEndpoint("test/swagger.json", "Dev Server Test API");
    c.RoutePrefix = "swagger";
    c.ConfigObject.AdditionalItems["https"] = true;
    c.EnableFilter();
});


app.MapControllers();

// await app.UseMongoSeedAsync();

app.Run();