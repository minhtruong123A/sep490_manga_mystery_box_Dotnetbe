using AspNetCoreRateLimit;
using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using DataAccessLayers.UnitOfWork;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SEP_MMB_API;
using Services.AutoMapper;
using Services.Interface;
using Services.Service;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.RegularExpressions;
using BusinessObjects.Dtos.PayOS;
using Net.payOS;
using System.Text.Json.Serialization;
using Services.Helper.Supabase;

var builder = WebApplication.CreateBuilder(args);
var devPassword = builder.Configuration["DevSettings:DevPassword"];

//appsettings.json setting PayOs
builder.Services.Configure<PayOSConfig>(builder.Configuration.GetSection("PayOS"));

//singleton PayOS
var config = builder.Configuration.GetSection("PayOS").Get<PayOSConfig>();
builder.Services.AddSingleton(new PayOS(config.ClientId, config.ApiKey, config.ChecksumKey));

//inject PayOSConfig
builder.Services.Configure<PayOSConfig>(builder.Configuration.GetSection("PayOS"));

//Supabase config
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase")
);
builder.Services.AddSingleton<ISupabaseStorageHelper, SupabaseStorageHelper>();

// enable Swagger to detect API endpoints
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); builder.Services.AddEndpointsApiExplorer();

//if (!builder.Environment.IsProduction())
//{
builder.Services.AddSwaggerGen(c =>
{
    //switch selection between user swagger and dev swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MMB Main API", Version = "v1" });
    c.SwaggerDoc("test", new OpenApiInfo { Title = "Server Test", Version = "v1" });
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;
        var groupName = apiDesc.GroupName ?? "v1";
        return docName == groupName;
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' [space] and then your token.\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
    });

    //open dev password to use server test api
    c.AddSecurityDefinition("MMB Dev Password", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "mmb-dev-password",
        Type = SecuritySchemeType.ApiKey,
        Description = "Enter MMB Dev Password to open Server Test API: ******"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "MMB Dev Password"
                    }
                },
                Array.Empty<string>()
            }
    });
});
//}

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Rate Limiting for preventing DDOS
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

//Csrf cookie
//builder.Services.AddAntiforgery(opt =>
//{
//    opt.Cookie.Name = "XSRF-TOKEN";
//    opt.HeaderName = "X-XSRF-TOKEN";
//});

//DbContext
builder.Services.AddSingleton<MongoDbContext>();

//Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
builder.Services.AddScoped<IMangaBoxRepository, MangaBoxRepository>();
builder.Services.AddScoped<ISellProductRepository, SellProductRepository>();
builder.Services.AddScoped<IUserCollectionRepository, UserCollectionRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
builder.Services.AddScoped<IUseDigitalWalletRepository, UseDigitalWalletRepository>();
builder.Services.AddScoped<IPayOSRepository, PayOSRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductInMangaBoxRepository,  ProductInMangaBoxRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IMysteryBoxRepository, MysteryBoxRepository>();
builder.Services.AddScoped<IBoxOrderRepository, BoxOrderRepository>();
builder.Services.AddScoped<IProductOrderRepository, ProductOrderRepository>();
builder.Services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
builder.Services.AddScoped<IDigitalPaymentSessionRepository, DigitalPaymentSessionRepository>();
builder.Services.AddScoped<IUserBoxRepository, UserBoxRepository>();
builder.Services.AddScoped<IUserProductRepository, UserProductRepository>();


//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMangaBoxService, MangaBoxService>();
builder.Services.AddScoped<ISellProductService, SellProductService>();
builder.Services.AddScoped<IUserCollectionService, UserCollectionService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IModerationService, ModerationService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();
builder.Services.AddScoped<IUseDigitalWalletService, UseDigitalWalletService>();
builder.Services.AddScoped<ISignedUrlService, SignedUrlService>();
builder.Services.AddScoped<IProductInMangaBoxService,  ProductInMangaBoxService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IOrderHistoryService, OrderHistoryService>();
builder.Services.AddScoped<IUserBoxService, UserBoxService>();
builder.Services.AddScoped<IUserProductService, UserProductService>();

//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:JWT_SECRET"])),
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});
var app = builder.Build();

//Seed DB: if you want to fake data, uncomment the line down below 
//using (var scope = app.Services.CreateScope())
//{
//    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
//    var seeder = new MongoSeeder(mongoDbContext);
//    await seeder.SeedAsync();
//}




//set permissions in middleware
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/api/test"))
    {
        var hasHeader = context.Request.Headers.TryGetValue("mmb-dev-password", out var password);
        if (!hasHeader || password != devPassword)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "MMB-Dev realm=\"Only system developers can access this endpoint.\"";
            await context.Response.WriteAsync("Unauthorized: You must provide a valid MMB Dev Password.");
            return;
        }
    }
    await next();
});

//MongoDB injection filter
app.Use(async (context, next) =>
{
    if (context.Request.Method is "POST" or "PUT" or "PATCH")
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var patterns = new[]
        {
            "<script>", "root", "ssh", "$ne", "$where", "$regex",
            "eval\\(", "function\\(", "sleep\\(", "drop\\s+collection", "db\\.getCollection"
        };

        foreach (var pattern in patterns)
        {
            if (Regex.IsMatch(body, pattern, RegexOptions.IgnoreCase))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Suspicious input: {pattern}");
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Blocked request: matched pattern {pattern}", pattern);
                return;
            }
        }
    }
    await next();
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseIpRateLimiting();

app.UseAuthentication();

// Middleware avoid refresh token call API
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    var hasAuthorizeAttribute = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
    if (hasAuthorizeAttribute)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var isRefreshToken = user.Claims.FirstOrDefault(c => c.Type == "is_refresh_token")?.Value;
            if (isRefreshToken == "true")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Refresh tokens are not allowed to access this API.");
                return;
            }
        }
    }

    await next();
});

app.UseAuthorization();

// Csrf cookie middleware
//app.Use(async (context, next) =>
//{
//    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
//    var hasXSRF = context.Request.Cookies.ContainsKey("XSRF-TOKEN");

//    if (hasXSRF &&
//        (HttpMethods.IsPost(context.Request.Method) ||
//         HttpMethods.IsPut(context.Request.Method) ||
//         HttpMethods.IsDelete(context.Request.Method))) await antiforgery.ValidateRequestAsync(context);

//    await next();
//});

// Configure the HTTP request pipeline.
//if (!app.Environment.IsProduction())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main User API");
    c.SwaggerEndpoint("/swagger/test/swagger.json", "Dev Server Test API");
    c.RoutePrefix = "swagger";
    c.ConfigObject.AdditionalItems["https"] = true;
    c.EnableFilter();
});
//}

app.MapControllers();

app.Run();
