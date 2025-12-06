using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pento.Infrastructure.External.Identity;

namespace Pento.API.Extensions;

internal static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                KeycloakOptions auth = context.ApplicationServices
                                  .GetRequiredService<IOptions<KeycloakOptions>>().Value;

                document.Info.Title = "Pento API";
                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Name = "JWT Authentication",
                        Description = "Enter your JWT token in this field",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    },
                    ["oauth2"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Description = "Authorization Code with PKCE via Keycloak",
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{auth.Authority}/protocol/openid-connect/auth"),
                                TokenUrl = new Uri(auth.TokenUrl),
                            }
                        }
                    }
                };

                document.SecurityRequirements =
                [

                    new OpenApiSecurityRequirement
                    {
                        [ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } } ]
                            = Array.Empty<string>()
                    }
                ];

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseOpenApiSwaggerUI(this WebApplication app)
    {
        app.MapOpenApi(); // serves /openapi/v1.json

        IConfiguration config = app.Configuration;
        string clientId = config["SWAGGERUI_CLIENTID"]
            ?? throw new InvalidOperationException("SWAGGERUI_CLIENTID is not configured");
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Pento API v1");
            options.OAuthClientId(clientId);
            options.OAuthUsePkce();
            options.EnablePersistAuthorization();
        });

        return app;
    }
}
