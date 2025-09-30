using System.Reflection;
using Pento.API;
using Pento.API.Extensions;
using Pento.API.Middleware;
using Pento.Application;
using Pento.Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication().AddInfrastructure(builder.Configuration).AddPresentation();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}
app.UseLogContext();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.MapEndpoints();

app.UseHttpsRedirection();

await app.RunAsync();

