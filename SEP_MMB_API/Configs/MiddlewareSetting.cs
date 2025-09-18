using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace SEP_MMB_API.Configs;

public static class MiddlewareSetting
{
    public static IApplicationBuilder UseDevPasswordProtection(this IApplicationBuilder app, string devPassword)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/test"))
            {
                var hasHeader = context.Request.Headers.TryGetValue("mmb-dev-password", out var password);
                if (!hasHeader || password != devPassword)
                {
                    context.Response.StatusCode = 401;
                    context.Response.Headers["WWW-Authenticate"] = "MMB-Dev realm=\"Only system developers can access this endpoint.\"";
                    await context.Response.WriteAsync("Unauthorized: Invalid MMB Dev Password.");
                    return;
                }
            }
            await next();
        });

        return app;
    }
    
    public static IApplicationBuilder UseMongoInjectionFilter(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Method is "POST" or "PUT" or "PATCH" &&
                !context.Request.ContentType?.StartsWith("multipart/form-data") == true)
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
                    if (!Regex.IsMatch(body, pattern, RegexOptions.IgnoreCase)) continue;
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"Suspicious input: {pattern}");
                    return;
                }
            }
            await next();
        });

        return app;
    }
    
    public static IApplicationBuilder UseRefreshTokenRestriction(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var endpoint = context.GetEndpoint();
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

            if (hasAuthorize && context.User.Identity?.IsAuthenticated == true)
            {
                var isRefresh = context.User.Claims.FirstOrDefault(c => c.Type == "is_refresh_token")?.Value;
                if (isRefresh == "true")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Refresh tokens are not allowed.");
                    return;
                }
            }

            await next();
        });

        return app;
    }
    
    public static IApplicationBuilder UseForwardConfig(this IApplicationBuilder app)
    {
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
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/cs", out var remaining))
            {
                context.Request.PathBase = "/cs";
                context.Request.Path = remaining;
            }

            await next();
        });
        return app;
    }
}