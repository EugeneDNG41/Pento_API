namespace Pento.API.Extensions;
#pragma warning disable S125
internal static class CorsExtensions
{
    public static IHostApplicationBuilder AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins(
                         "http://localhost:3000",   // your dev SPA
                         "https://localhost:3000",
                         "http://localhost:5278",
                         "https://localhost:7130")
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    //if (builder.Environment.IsDevelopment())
                    //{
                    //    policy.AllowAnyOrigin();
                    //}
                    //else
                    //{
                    //    string originsString = builder.Configuration["AllowedOrigins"] ?? string.Empty;
                    //    string[] allowedOrigins = originsString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    //    policy.WithOrigins(allowedOrigins);
                    //}

                    //policy.WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType).AllowAnyMethod();
                });
        });

        return builder;
    }
}
