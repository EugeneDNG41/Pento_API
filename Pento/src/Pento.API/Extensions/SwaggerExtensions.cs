using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Quartz.Impl.AdoJobStore.Common;

namespace Pento.API.Extensions;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            };

            options.AddSecurityRequirement(securityRequirement);
            options.SwaggerDoc("PentoApp", new OpenApiInfo
            {
                Title = "Pento API",
                Version = "v1",
                Description = "Pento API built using Clean Architecture."
            });

            options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
        });

        return services;
    }
    internal static WebApplication UseSwaggerRoute(this WebApplication app)
    {
        IConfiguration cfg = app.Configuration;
        string clientId = cfg["SWAGGERUI_CLIENTID"]
            ?? throw new InvalidOperationException("SWAGGERUI_CLIENTID is not configured");
        app.UseSwagger();
        app.UseStaticFiles();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PentoApp");
            options.OAuthClientId(clientId);
            options.OAuthUsePkce();
            options.EnablePersistAuthorization();
        });
        return app;
    }
}
