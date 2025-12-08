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
                        policy.WithOrigins("https://localhost:7130", "http://localhost:5278");
                    }
                    else
                    {
                        string originsString = builder.Configuration["AllowedOrigins"] ?? string.Empty;
                        string[] allowedOrigins = originsString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        policy.WithOrigins(allowedOrigins);
                    }

                    policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
        });

        return builder;
    }
}
