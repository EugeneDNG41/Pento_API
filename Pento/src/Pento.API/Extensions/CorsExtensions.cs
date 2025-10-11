using Microsoft.Net.Http.Headers;

namespace Pento.API.Extensions;

internal static class CorsExtensions
{
    public static IHostApplicationBuilder AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.AllowAnyOrigin();
                    }
                    else
                    {
                        string originsString = builder.Configuration["AllowedOrigins"] ?? string.Empty;
                        string[] allowedOrigins = originsString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        policy.WithOrigins(allowedOrigins);
                    }

                    policy.WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                          .AllowAnyMethod();
                });
        });

        return builder;
    }
}
