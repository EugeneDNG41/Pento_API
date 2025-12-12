using System.Text.Json.Serialization;
using Dapper;
using FirebaseAdmin;
using GenerativeAI;
using Google.Apis.Auth.OAuth2;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.External.Barcode;
using Pento.Application.Abstractions.External.Email;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.External.Vision;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Caching;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Infrastructure.Authentication;
using Pento.Infrastructure.Authorization;
using Pento.Infrastructure.Configurations;
using Pento.Infrastructure.External.AI;
using Pento.Infrastructure.External.Email;
using Pento.Infrastructure.External.File;
using Pento.Infrastructure.External.Firebase;
using Pento.Infrastructure.External.Google;
using Pento.Infrastructure.External.Identity;
using Pento.Infrastructure.External.OpenFoodFacts;
using Pento.Infrastructure.External.PayOS;
using Pento.Infrastructure.External.Quartz;
using Pento.Infrastructure.External.Vision;
using Pento.Infrastructure.Persistence;
using Pento.Infrastructure.Persistence.TypeHandlers;
using Pento.Infrastructure.Services;
using Pento.Infrastructure.Utility.Caching;
using Pento.Infrastructure.Utility.Clock;
using Pento.Infrastructure.Utility.Converter;
using Pento.Infrastructure.Utility.Outbox;
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
        services.AddScoped<IEntitlementService, EntitlementService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IMilestoneService, MilestoneService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<TradeService>();
        services.AddHttpClient<OpenFoodFactsClient>((httpClient) =>
        {
            httpClient.BaseAddress = new Uri("https://world.openfoodfacts.net/api/v2/");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Pento/1.0 (smarthouseholdfoodpento@gmail.com)");
        });
        services.AddScoped(sp =>
        {
            string apiKey = configuration["Gemini:ApiKey"];
            var googleAI = new GoogleAi(apiKey);

            GenerativeModel model = googleAI.CreateGenerativeModel("models/gemini-1.5-flash");

            return model;
        });
        
        GoogleOptions googleOptions = configuration.GetRequiredSection("Google")
         .Get<GoogleOptions>()
         ?? throw new InvalidOperationException("Google section is missing or invalid");
        string jsonOptions = Newtonsoft.Json.JsonConvert.SerializeObject(googleOptions);
        ServiceAccountCredential cred = CredentialFactory.FromJson<ServiceAccountCredential>(jsonOptions);
        var firebaseApp = FirebaseApp.Create(new AppOptions()
        {
            Credential = cred.ToGoogleCredential()
        });
        services.AddSingleton(firebaseApp);
        
        services.AddSingleton<IUserIdProvider, HubUserIdProvider>();
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


        services.AddScoped<IFoodAiEnricher, GeminiFoodAiEnricher>();
        services.AddScoped<IFoodImageGenerator, ImagenFoodImageGenerator>();
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IUnsplashImageService, UnsplashImageService>();
        services.AddScoped<IPixabayImageService, PixabayImageService>();
        services.AddScoped<IVisionService, VisionService>();
        services.AddScoped<INotificationService, FcmNotificationService>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string redisConnectionString = configuration.GetConnectionStringOrThrow("redis");
        services.AddSignalR().AddStackExchangeRedis(redisConnectionString);
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
        KeycloakOptions keycloakOptions = builder.Configuration.GetRequiredSection("Keycloak")
            .Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Keycloak section is missing or invalid");
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
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<QuartzJobsSetup>();
    }
}
