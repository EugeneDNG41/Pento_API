using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Npgsql;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Caching;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Email;
using Pento.Application.Abstractions.Identity;
using Pento.Common.Infrastructure.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;
using Pento.Infrastructure.Authorization;
using Pento.Infrastructure.Caching;
using Pento.Infrastructure.Configurations;
using Pento.Infrastructure.Data;
using Pento.Infrastructure.Email;
using Pento.Infrastructure.Identity;
using Pento.Infrastructure.Outbox;
using Pento.Infrastructure.Repositories;
using Quartz;

namespace Pento.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();
        
        AddPersistence(services, configuration);

        AddCaching(services, configuration);

        AddAuthenticationAndAuthorization(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionStringOrThrow("pento-db");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
        services.TryAddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string redisConnectionString = configuration.GetConnectionStringOrThrow("redis");
        try
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }
        services.AddSingleton<ICacheService, CacheService>();
    }
    private static void AddAuthenticationAndAuthorization(IServiceCollection services, IConfiguration configuration)
    {
        KeycloakOptions keycloakOptions = configuration.GetRequiredSection("Keycloak").Get<KeycloakOptions>() ?? throw new InvalidOperationException("Keycloak section is missing or invalid");

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));
        services.AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection("Keycloak"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<KeyCloakAuthDelegatingHandler>();
        
        services
            .AddHttpClient<KeyCloakClient>((httpClient) =>
            {
                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();
        services.AddHttpClient<IJwtService, JwtService>((httpClient) =>
        {
            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer("keycloak", realm: "pento", opt =>
        {
            opt.RequireHttpsMetadata = false;
            opt.MapInboundClaims = false;
            opt.Audience = keycloakOptions.ClientId;
            opt.Authority = keycloakOptions.Authority;
            opt.MetadataAddress = $"{keycloakOptions.Authority}/.well-known/openid-configuration"
            ;
        });
        services.AddAuthorizationBuilder();
        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IPermissionService, PermissionService>();

        services.AddTransient<IIdentityProviderService, IdentityProviderService>();
    }
    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }
}
