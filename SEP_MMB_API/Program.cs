using BusinessObjects;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using DataAccessLayers.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SEP_MMB_API;
using Services.AutoMapper;
using Services.Interface;
using Services.Service;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var devPassword = builder.Configuration["DevSettings:DevPassword"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

//DbContext
builder.Services.AddSingleton<MongoDbContext>();

//Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();

//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:JWT_SECRET"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var error = context.Exception;
            return Task.CompletedTask;
        }
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

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
app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Post ||
        context.Request.Method == HttpMethods.Put ||
        context.Request.Method == HttpMethods.Patch)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, 
                                            detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        var forbiddenKeywords = new[]
        {
            "<script>","root","ssh","$ne","$where"
        };
        var match = forbiddenKeywords.FirstOrDefault(keyword => body.Contains(keyword,StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Request blocked: forbidden keyword using for request \"{match}\" is not allowed.");
            return;
        }
    }
    await next();
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main User API");
    c.SwaggerEndpoint("/swagger/test/swagger.json", "Dev Server Test API");
    c.EnableFilter();
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
