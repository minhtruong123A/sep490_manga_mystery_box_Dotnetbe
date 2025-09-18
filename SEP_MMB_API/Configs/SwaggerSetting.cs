using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SEP_MMB_API.Configs;

public static class SwaggerSetting
{
public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); 

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
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

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/cs/swagger/v1/swagger.json", "Main User API");
            c.SwaggerEndpoint("/cs/swagger/test/swagger.json", "Dev Server Test API");
            c.RoutePrefix = "swagger";
            c.ConfigObject.AdditionalItems["https"] = true;
            c.EnableFilter();
        });
        
        //app.UseSwagger();
        //app.UseSwaggerUI(c =>
        //{
        //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main User API");
        //    c.SwaggerEndpoint("/swagger/test/swagger.json", "Dev Server Test API");
        //    c.RoutePrefix = "swagger";
        //    c.ConfigObject.AdditionalItems["https"] = true;
        //    c.EnableFilter();
        //});

        return app;
    }
}