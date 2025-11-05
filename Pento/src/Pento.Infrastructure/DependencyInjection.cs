using System.Text.Json.Serialization;
using Dapper;
using ImTools;
using JasperFx;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Caching;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Email;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.Identity;
using Pento.Application.FoodItems.Projections;
using Pento.Common.Infrastructure.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.BlogPosts;
using Pento.Domain.Comments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.MealPlans;
using Pento.Domain.PossibleUnits;
using Pento.Domain.RecipeDirections;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;
using Pento.Domain.Users;
using Pento.Infrastructure.AI;
using Pento.Infrastructure.Authentication;
using Pento.Infrastructure.Authorization;
using Pento.Infrastructure.Caching;
using Pento.Infrastructure.Configurations;
using Pento.Infrastructure.Converter;
using Pento.Infrastructure.Data;
using Pento.Infrastructure.Email;
using Pento.Infrastructure.File;
using Pento.Infrastructure.Identity;
using Pento.Infrastructure.Outbox;
using Pento.Infrastructure.Repositories;
using Quartz;
using Weasel.Core;

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

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionStringOrThrow("pento-db");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.Events.StreamIdentity = StreamIdentity.AsGuid;
            options.UseSystemTextJsonForSerialization(EnumStorage.AsString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Projections.Errors.SkipApplyErrors = false;
            options.Projections.Errors.SkipSerializationErrors = false;
            options.Projections.Errors.SkipUnknownEvents = false;


            options.Projections.LiveStreamAggregation<FoodItem>();
            options.Projections.Add<FoodItemDetailProjection>(ProjectionLifecycle.Inline);

            options.Projections.UseIdentityMapForAggregates = true;
            // Event metadata
            options.Events.MetadataConfig.UserNameEnabled = true;
        })
        .ApplyAllDatabaseChangesOnStartup()
        .UseLightweightSessions()
        .AddAsyncDaemon(DaemonMode.HotCold);

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
        services.TryAddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());
        SqlMapper.AddTypeHandler(new UriTypeHandler());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IUnitConverter, UnitConverter>();


        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFoodReferenceRepository, FoodReferenceRepository>();
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<IGiveawayClaimRepository, GiveawayClaimRepository>();
        services.AddScoped<IGiveawayPostRepository, GiveawayPostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
        services.AddScoped<IRecipeDirectionRepository,RecipeDirectionRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IPossibleUnitRepository, PossibleUnitRepository>();
        services.AddScoped<IFoodAiEnricher, GeminiFoodAiEnricher>();
        services.AddScoped<IFoodImageGenerator, ImagenFoodImageGenerator>();
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IUnsplashImageService, UnsplashImageService>();
        services.AddScoped<IPixabayImageService,PixabayImageService>();

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
