using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

namespace Pento.Infrastructure.Authentication;

internal sealed class JwtBearerConfigureOptions(IConfiguration configuration)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigurationSectionName = "Authentication";

    public void Configure(JwtBearerOptions options)
    {
        configuration.GetSection(ConfigurationSectionName).Bind(options);

        options.Events ??= new JwtBearerEvents();

        options.Events.OnMessageReceived = context =>
        {
            StringValues accessToken = context.Request.Query["access_token"];
            PathString path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken)
                && path.StartsWithSegments("/message-hub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}
public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        Func<MessageReceivedContext, Task> originalOnMessageReceived = options.Events.OnMessageReceived;
        options.Events.OnMessageReceived = async context =>
        {
            await originalOnMessageReceived(context);

            if (string.IsNullOrEmpty(context.Token))
            {
                StringValues accessToken = context.Request.Query["access_token"];
                PathString requestPath = context.HttpContext.Request.Path;
                string endPoint = $"/message-hub";

                if (!string.IsNullOrEmpty(accessToken) &&
                    requestPath.StartsWithSegments(endPoint))
                {
                    context.Token = accessToken;
                }
            }
        };
    }
}
