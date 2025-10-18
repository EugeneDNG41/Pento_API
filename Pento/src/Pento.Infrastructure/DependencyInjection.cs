using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
using Pento.Domain.BlogPosts;
using Pento.Domain.Comments;
using Pento.Domain.FoodReferences;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.MealPlanItems;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.StorageItems;
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
        services.AddScoped<IFoodReferenceRepository, FoodReferenceRepository>();
        services.AddScoped<IStorageItemRepository, StorageItemRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IMealPlanItemRepository, MealPlanItemRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<IGiveawayClaimRepository, GiveawayClaimRepository>();
        services.AddScoped<IGiveawayPostRepository, GiveawayPostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
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
    public static WebApplicationBuilder AddAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        KeycloakOptions keycloakOptions = builder.Configuration.GetRequiredSection("Keycloak").Get<KeycloakOptions>() ?? throw new InvalidOperationException("Keycloak section is missing or invalid");

        builder.Services.AddOptions<KeycloakOptions>()
            .Bind(builder.Configuration.GetSection("Keycloak"))
            .PostConfigure(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    return;
                }
                options.Authority = options.Authority.Replace("http://", "https://");
                options.TokenUrl = options.TokenUrl.Replace("http://", "https://");
                options.AdminUrl = options.AdminUrl.Replace("http://", "https://");
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddTransient<KeyCloakAuthDelegatingHandler>();
        string keycloakAuthority = builder.Environment.IsDevelopment() ? keycloakOptions.Authority : keycloakOptions.Authority.Replace("http://", "https://");
        builder.Services
            .AddHttpClient<KeyCloakClient>((httpClient) =>
            {
                httpClient.BaseAddress = new Uri($"{keycloakAuthority}/admin/realms/pento/");
            })
            .AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();
        builder.Services.AddHttpClient<IJwtService, JwtService>((httpClient) =>
        {
            httpClient.BaseAddress = new Uri($"{keycloakAuthority}/realms/pento/protocol/openid-connect/token");
        });
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer("keycloak", realm: "pento", opt =>
        {
            if (builder.Environment.IsDevelopment())
            {
                opt.RequireHttpsMetadata = false;
            }          
            opt.MapInboundClaims = false;
            opt.Audience = keycloakOptions.ClientId;
            opt.Authority = keycloakAuthority;
            opt.MetadataAddress = $"{keycloakAuthority}/.well-known/openid-configuration"
            ;
        });
        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUserContext, UserContext>();

        builder.Services.AddScoped<IPermissionService, PermissionService>();

        builder.Services.AddTransient<IIdentityProviderService, IdentityProviderService>();
        return builder;
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
