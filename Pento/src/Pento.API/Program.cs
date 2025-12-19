using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Pento.API;
using Pento.API.Extensions;
using Pento.API.Middleware;
using Pento.Application;
using Pento.Application.Abstractions.Messaging;
using Pento.Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHealthChecks();
builder
    .AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment())
    .AddApplication()   
    .AddPresentation()
    .AddEndpoints(Assembly.GetExecutingAssembly());

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
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
     app.ApplyMigrations();
}


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

#pragma warning disable CA1515 // Consider making public types internal
public partial class Program { }

