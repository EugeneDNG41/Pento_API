using System.Text.Json.Serialization;
using Dapper;
using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using PayOS;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Caching;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Email;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.OpenFoodFacts;
using Pento.Application.Abstractions.PayOS;
using Pento.Application.Abstractions.Vision;
using Pento.Domain.Abstractions;
using Pento.Domain.BlogPosts;
using Pento.Domain.Comments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeDirections;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;
using Pento.Domain.Users;
using Pento.Infrastructure.AI;
using Pento.Infrastructure.Authentication;
using Pento.Infrastructure.Authorization;
using Pento.Infrastructure.Caching;
using Pento.Infrastructure.Clock;
using Pento.Infrastructure.Configurations;
using Pento.Infrastructure.Converter;
using Pento.Infrastructure.Data;
using Pento.Infrastructure.Email;
using Pento.Infrastructure.File;
using Pento.Infrastructure.Identity;
using Pento.Infrastructure.OpenFoodFacts;
using Pento.Infrastructure.Outbox;
using Pento.Infrastructure.PayOS;
using Pento.Infrastructure.Repositories;
using Pento.Infrastructure.Vision;
using Quartz;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace Pento.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();
        services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        AddPersistence(services, configuration);

        AddCaching(services, configuration);

        AddBackgroundJobs(services, configuration);

        services.AddScoped<IBarcodeService, BarcodeService>();
        services.AddScoped<IConverterService, ConverterService>();
        services.AddSingleton(sp =>
        {
            string apiKey = configuration["Gemini:ApiKey"];
            var googleAI = new GoogleAi(apiKey);

            GeminiModel model = googleAI.CreateGeminiModel("gemini-2.0-flash");

            return model;
        });
#pragma warning disable S1481 // Unused local variables should be removed
        PayOSCustomOptions payosOptions = configuration.GetRequiredSection("PayOS").Get<PayOSCustomOptions>() ?? throw new InvalidOperationException("Keycloak section is missing or invalid");
#pragma warning restore S1481 // Unused local variables should be removed

        services.AddOptions<PayOSCustomOptions>()
            .Bind(configuration.GetSection("PayOS"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IPayOSService, PayOSService>();
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
        SqlMapper.AddTypeHandler(new UriTypeHandler());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        


        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFoodReferenceRepository, FoodReferenceRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<IGiveawayClaimRepository, GiveawayClaimRepository>();
        services.AddScoped<IGiveawayPostRepository, GiveawayPostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
        services.AddScoped<IRecipeDirectionRepository,RecipeDirectionRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IFoodAiEnricher, GeminiFoodAiEnricher>();
        services.AddScoped<IFoodImageGenerator, ImagenFoodImageGenerator>();
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IUnsplashImageService, UnsplashImageService>();
        services.AddScoped<IPixabayImageService,PixabayImageService>();
        services.AddScoped<IVisionService, VisionService>();

    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string redisConnectionString = configuration.GetConnectionStringOrThrow("redis");
        using var redis = new RedisCache(new RedisCacheOptions
        {
            Configuration = redisConnectionString
        });

        services.AddFusionCache()
        .WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            Duration = TimeSpan.FromHours(1),

            // FACTORY TIMEOUT
            FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
            FactoryHardTimeout = TimeSpan.FromMilliseconds(1500),

            // FAILSAFE
            IsFailSafeEnabled = true,
            FailSafeMaxDuration = TimeSpan.FromHours(2),
            FailSafeThrottleDuration = TimeSpan.FromSeconds(30)
        })
        .WithSerializer(new FusionCacheNewtonsoftJsonSerializer())
        .WithDistributedCache(redis);
        services.AddSingleton<ICacheService, CacheService>();
    }
    public static WebApplicationBuilder AddAspireHostedServices(this WebApplicationBuilder builder)
    {
        
#pragma warning disable S125 // Sections of code should not be commented out
                            //builder.AddSeqEndpoint("seq");
        builder.AddAzureBlobServiceClient("blobs");
        return builder;
#pragma warning restore S125 // Sections of code should not be commented out
    }
    public static WebApplicationBuilder AddAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPermissionService, PermissionService>();
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
        builder.Services.AddFluentEmail("");
        builder.Services.AddScoped<IEmailService, EmailService>();
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
            httpClient.BaseAddress = new Uri($"{keycloakAuthority}/realms/pento/protocol/openid-connect/");
        });
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
        {
            if (builder.Environment.IsDevelopment())
            {
                opt.RequireHttpsMetadata = false;
            }
            opt.MapInboundClaims = false;
            opt.Authority = keycloakAuthority;
            opt.Audience = keycloakOptions.ClientId;         
            opt.TokenValidationParameters.ValidIssuer = $"{keycloakAuthority}/realms/pento";
            opt.MetadataAddress = $"{keycloakAuthority}/realms/pento/.well-known/openid-configuration";      
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUserContext, UserContext>();

        builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        builder.Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
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
            configurator.UsePersistentStore(options =>
            {
                options.UsePostgres(sqlServerOptions =>
                {
                    sqlServerOptions.ConnectionString = configuration.GetConnectionStringOrThrow("pento-db");
                    sqlServerOptions.TablePrefix = "quartz_";
                });
                options.UseClustering(clusterOptions =>
                {
                    clusterOptions.CheckinInterval = TimeSpan.FromSeconds(20);
                });
                options.UseProperties = true;
            });
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }
}
