using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Pento.API;
using Pento.API.Extensions;
using Pento.API.Middleware;
using Pento.Application;
using Pento.Infrastructure;
using Pento.Infrastructure.Services;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHealthChecks();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation()
    .AddEndpoints(Assembly.GetExecutingAssembly());
builder.AddAuthenticationAndAuthorization().AddAspireHostedServices();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});
builder.AddCors();
WebApplication app = builder.Build();
app.ApplyMigrations();

app.UseExceptionHandler();

app.UseRouting();

app.UseCors();

app.MapDefaultEndpoints();

app.UseSwaggerRoute();

app.UseLogContext();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<MessageHub>("message-hub");

app.MapEndpoints();

await app.RunAsync();

