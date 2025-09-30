using System.Reflection;
using Pento.API.Extensions;
using Pento.API.Middleware;
using Pento.Common.Application;
using Pento.Common.Presentation.Endpoints;
using Pento.Modules.Pantry.Application;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

Assembly[] moduleAssemblies = new[]
{
    Pento.Modules.Pantry.Application.AssemblyReference.Assembly
};

builder.Services.AddApplication(moduleAssemblies);

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

