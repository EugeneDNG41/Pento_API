using System.Text.Json.Serialization;
using Dapper;
using FirebaseAdmin;
using GenerativeAI;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.External.Barcode;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Infrastructure.Authentication;
using Pento.Infrastructure.Authorization;
using Pento.Infrastructure.Configurations;
using Pento.Infrastructure.External.AI;
using Pento.Infrastructure.External.File;
using Pento.Infrastructure.External.Firebase;
using Pento.Infrastructure.External.Google;
using Pento.Infrastructure.External.Identity;
using Pento.Infrastructure.External.OpenFoodFacts;
using Pento.Infrastructure.External.PayOS;
using Pento.Infrastructure.External.Quartz;
using Pento.Infrastructure.Persistence;
using Pento.Infrastructure.Persistence.Seed;
using Pento.Infrastructure.Persistence.TypeHandlers;
using Pento.Infrastructure.Services;
using Pento.Infrastructure.Utility.Clock;
using Pento.Infrastructure.Utility.Converter;
using Pento.Infrastructure.Utility.Outbox;
using Quartz;
using ZiggyCreatures.Caching.Fusion;

namespace Pento.Infrastructure;

public static class DependencyInjection
{
    // =========================
    // ENTRY POINT
    // =========================
    public static IServiceCollection AddInfrastructure(
        this WebApplicationBuilder builder,
        IConfiguration configuration,
        bool isDevelopment)
    {
        string dbConnectionString = configuration.GetConnectionStringOrThrow("pento-db");
        string redisConnectionString = configuration.GetConnectionStringOrThrow("redis");
        builder.Services
            .AddCore()
            .AddSerialization()
            .AddPersistence(dbConnectionString)
            .AddCaching(redisConnectionString)
            .AddAuthenticationAndAuthorization(configuration, isDevelopment)
            .AddExternalApis(configuration)
            .AddApplicationServices()
            .AddBackgroundJobs(configuration);
        builder.AddAspireHostedServices();
        builder.Services.AddSignalR().AddStackExchangeRedis(redisConnectionString);

        return builder.Services;
    }

    // =========================
    // CORE / CROSS-CUTTING
    // =========================
    private static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        return services;
    }

    private static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services
            .Configure<JsonOptions>(o =>
                o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    // =========================
    // PERSISTENCE
    // =========================
    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(dataSource);
        services.TryAddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());
        SqlMapper.AddTypeHandler(new UriTypeHandler());

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // AI / Media / Notifications
        services.AddScoped<IFoodAiEnricher, GeminiFoodAiEnricher>();
        services.AddScoped<IFoodImageGenerator, ImagenFoodImageGenerator>();
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IUnsplashImageService, UnsplashImageService>();
        services.AddScoped<IPixabayImageService, PixabayImageService>();
        services.AddScoped<INotificationService, FcmNotificationService>();

        return services;
    }

    // =========================
    // APPLICATION SERVICES
    // =========================
    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBarcodeService, BarcodeService>();
        services.AddScoped<IConverterService, ConverterService>();
        services.AddScoped<IEntitlementService, EntitlementService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IMilestoneService, MilestoneService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<ICompartmentService, CompartmentService>();
        services.AddScoped<ITradeService, TradeService>();

        services.AddScoped<DataSeeder>();

        return services;
    }

    // =========================
    // EXTERNAL APIS & SDKs
    // =========================
    private static IServiceCollection AddExternalApis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // OpenFoodFacts
        services.AddHttpClient<OpenFoodFactsClient>(client =>
        {
            client.BaseAddress = new Uri("https://world.openfoodfacts.net/api/v2/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Pento/1.0 (smarthouseholdfoodpento@gmail.com)");
        });

        // Gemini
        services.AddScoped(sp =>
        {
            string? apiKey = configuration["Gemini:ApiKey"];
            var googleAI = new GoogleAi(apiKey);
            return googleAI.CreateGenerativeModel("models/gemini-1.5-flash");
        });

        // Firebase
        GoogleOptions googleOptions = configuration.GetRequiredSection("Google")
            .Get<GoogleOptions>()
            ?? throw new InvalidOperationException("Google section is missing or invalid");

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(googleOptions);
        var credential = CredentialFactory
            .FromJson<ServiceAccountCredential>(json)
            .ToGoogleCredential();

        services.AddSingleton(FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        }));

        // PayOS
        services.AddOptions<PayOSCustomOptions>()
            .Bind(configuration.GetSection("PayOS"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IPayOSService, PayOSService>();



        services.AddSingleton<IUserIdProvider, HubUserIdProvider>();

        return services;
    }

    // =========================
    // BACKGROUND JOBS
    // =========================
    private static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddQuartz(cfg =>
        {
            var id = Guid.NewGuid();
            cfg.SchedulerId = $"default-id-{id}";
            cfg.SchedulerName = $"default-name-{id}";
        });

        services.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        services.ConfigureOptions<QuartzJobsSetup>();

        return services;
    }

    // =========================
    // ASPIRE
    // =========================
    public static WebApplicationBuilder AddAspireHostedServices(
        this WebApplicationBuilder builder)
    {
        builder.AddAzureBlobServiceClient("blobs");
        return builder;
    }

    // =========================
    // AUTH & AUTHZ
    // =========================
    public static IServiceCollection AddAuthenticationAndAuthorization(
        this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();

        services
            .AddTransient<IClaimsTransformation, CustomClaimsTransformation>()
            .AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>()
            .AddTransient<IIdentityProviderService, IdentityProviderService>();

        KeycloakOptions keycloakOptions = configuration
            .GetRequiredSection("Keycloak")
            .Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Keycloak section is missing");

        services.AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection("Keycloak"))
            .PostConfigure(o =>
            {
                if (!isDevelopment)
                {
                    o.Authority = o.Authority.Replace("http://", "https://");
                    o.TokenUrl = o.TokenUrl.Replace("http://", "https://");
                    o.AdminUrl = o.AdminUrl.Replace("http://", "https://");
                }
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        string authority = isDevelopment
            ? keycloakOptions.Authority
            : keycloakOptions.Authority.Replace("http://", "https://");

        services.AddTransient<KeyCloakAuthDelegatingHandler>();

        services.AddHttpClient<KeyCloakClient>(c =>
        {
            c.BaseAddress = new Uri($"{authority}/admin/realms/pento/");
        }).AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>(c =>
        {
            c.BaseAddress = new Uri(
                $"{authority}/realms/pento/protocol/openid-connect/");
        });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = !isDevelopment;
                opt.MapInboundClaims = false;
                opt.Authority = authority;
                opt.Audience = keycloakOptions.ClientId;
                opt.TokenValidationParameters.ValidIssuer =
                    $"{authority}/realms/pento";
                opt.MetadataAddress =
                    $"{authority}/realms/pento/.well-known/openid-configuration";
            });

        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services, string redisConnectionString)
    {     
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });
        services.AddFusionCacheNewtonsoftJsonSerializer();

        services.AddFusionCacheStackExchangeRedisBackplane(options =>
        {
            options.Configuration = redisConnectionString;
        });
        services.AddFusionCache()
        .WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            Duration = TimeSpan.FromHours(1),

            IsFailSafeEnabled = true,
            FailSafeMaxDuration = TimeSpan.FromHours(2),
            FailSafeThrottleDuration = TimeSpan.FromSeconds(30),

            EagerRefreshThreshold = 0.9f,

            FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
            FactoryHardTimeout = TimeSpan.FromMilliseconds(1500)
        }).WithRegisteredDistributedCache().WithRegisteredSerializer().WithRegisteredBackplane().AsHybridCache();
        return services;
    }
   
}
