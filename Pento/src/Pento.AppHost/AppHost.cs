using Aspire.Hosting.Azure;
using Aspire.Hosting.Testing;
using Azure.Provisioning;
using Azure.Provisioning.PostgreSql;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SeqResource> seq = builder.AddSeq("seq")
    .ExcludeFromManifest()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

IResourceBuilder<RedisResource> cache = builder.AddRedis("redis");

IResourceBuilder<AzureStorageResource> storage = builder.AddAzureStorage("storage");
IResourceBuilder<AzureBlobStorageResource> blobs = storage.AddBlobs("blobs");

IResourceBuilder<AzurePostgresFlexibleServerResource> postgres = builder
    .AddAzurePostgresFlexibleServer("postgres")
    .RunAsContainer(postgres =>
    {
        postgres.WithBindMount("VolumeMount.AppHost-postgres-data", "/var/lib/postgresql/data");
        postgres.WithPgAdmin(pgAdmin =>
        {
            pgAdmin.WithHostPort(5050);
        });
    })
    .ConfigureInfrastructure(infra =>
    {
        PostgreSqlFlexibleServer pg = infra.GetProvisionableResources()
                      .OfType<PostgreSqlFlexibleServer>()
                      .Single();

        infra.Add(new ProvisioningOutput("postgresHost", typeof(string))
        {
            Value = pg.FullyQualifiedDomainName
        });
    });

IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> pentoDb = postgres.AddDatabase("pento-db", "pento");

IResourceBuilder<ParameterResource> keycloakPassword = builder.AddParameter("KeycloakPassword", secret: true, value: "admin");
IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", adminPassword: keycloakPassword)
                      .WithLifetime(ContainerLifetime.Persistent);

if (builder.ExecutionContext.IsRunMode)
{
    storage.RunAsEmulator(azurite => azurite.WithDataVolume());
    keycloak.WithDataVolume()
            .WithRealmImport("./realms");
}
if (builder.ExecutionContext.IsPublishMode)
{
    IResourceBuilder<ParameterResource> postgresUser = builder.AddParameter("PostgresUser", value: "postgres");
    IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
    postgres.WithPasswordAuthentication(userName: postgresUser, password: postgresPassword);

    IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> keycloakDb = postgres.AddDatabase("keycloak-db", "keycloak");

    var keycloakDbUrl = ReferenceExpression.Create(
        $"jdbc:postgresql://{postgres.GetOutput("postgresHost")}/{keycloakDb.Resource.DatabaseName}"
    );

    keycloak.WithEnvironment("KC_HTTP_ENABLED", "true")
            .WithEnvironment("KC_PROXY_HEADERS", "xforwarded")
            .WithEnvironment("KC_HOSTNAME_STRICT", "false")
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL", keycloakDbUrl)
            .WithEnvironment("KC_DB_USERNAME", postgresUser)
            .WithEnvironment("KC_DB_PASSWORD", postgresPassword)
            .WithEndpoint("http", e => e.IsExternal = true);
}

var keycloakAuthority = ReferenceExpression.Create(
    $"{keycloak.GetEndpoint("http").Property(EndpointProperty.Url)}");
var keycloakAdminUrl = ReferenceExpression.Create(
    $"{keycloak.GetEndpoint("http").Property(EndpointProperty.Url)}/admin/realms/pento/");
var keycloakTokenUrl = ReferenceExpression.Create(
    $"{keycloak.GetEndpoint("http").Property(EndpointProperty.Url)}/realms/pento/protocol/openid-connect/token");

IResourceBuilder<ParameterResource> keycloakClientId = builder.AddParameter("KeycloakClientId");
IResourceBuilder<ParameterResource> keycloakClientSecret = builder.AddParameter("KeycloakClientSecret", secret: true);
IResourceBuilder<ParameterResource> geminiApiKey = builder.AddParameter("GeminiApiKey", secret: true);
IResourceBuilder<ParameterResource> pixabayApiKey = builder.AddParameter("PixabayApiKey", secret: true);
IResourceBuilder<ParameterResource> visionEndpoint = builder.AddParameter("VisionEndpoint");
IResourceBuilder<ParameterResource> visionKey = builder.AddParameter("VisionApiKey", secret: true);
IResourceBuilder<ParameterResource> payosClientId = builder.AddParameter("PayosClientId", secret: true);
IResourceBuilder<ParameterResource> payosApiKey = builder.AddParameter("PayosApiKey", secret: true);
IResourceBuilder<ParameterResource> payosChecksumKey = builder.AddParameter("PayosChecksumKey", secret: true);
IResourceBuilder<ParameterResource> googleType = builder.AddParameter("GoogleType");
IResourceBuilder<ParameterResource> googleProjectId = builder.AddParameter("GoogleProjectId");
IResourceBuilder<ParameterResource> googlePrivateKeyId = builder.AddParameter("GooglePrivateKeyId");
IResourceBuilder<ParameterResource> googlePrivateKey = builder.AddParameter("GooglePrivateKey", secret: true);
IResourceBuilder<ParameterResource> googleClientEmail = builder.AddParameter("GoogleClientEmail");
IResourceBuilder<ParameterResource> googleClientId = builder.AddParameter("GoogleClientId");
IResourceBuilder<ParameterResource> googleAuthUri = builder.AddParameter("GoogleAuthUri");
IResourceBuilder<ParameterResource> googleTokenUri = builder.AddParameter("GoogleTokenUri");
IResourceBuilder<ParameterResource> googleAuthProviderCertUrl = builder.AddParameter("GoogleAuthProviderX509CertUrl");
IResourceBuilder<ParameterResource> googleClientCertUrl = builder.AddParameter("GoogleClientX509CertUrl");
IResourceBuilder<ParameterResource> googleUniverseDomain = builder.AddParameter("GoogleUniverseDomain");

IResourceBuilder<ProjectResource> project = builder.AddProject<Projects.Pento_API>("pento-api")
    .WithExternalHttpEndpoints()
    .WithEnvironment("Keycloak__Authority", keycloakAuthority)
    .WithEnvironment("Keycloak__AdminUrl", keycloakAdminUrl)
    .WithEnvironment("Keycloak__TokenUrl", keycloakTokenUrl)
    .WithEnvironment("Keycloak__ClientId", keycloakClientId)
    .WithEnvironment("Keycloak__ClientSecret", keycloakClientSecret)
    .WithEnvironment("Gemini__ApiKey", geminiApiKey)
    .WithEnvironment("Pixabay__ApiKey", pixabayApiKey)
    .WithEnvironment("Vision__Endpoint", visionEndpoint)
    .WithEnvironment("Vision__ApiKey", visionKey)
    .WithEnvironment("PayOS__ClientId", payosClientId)
    .WithEnvironment("PayOS__ApiKey", payosApiKey)
    .WithEnvironment("PayOS__ChecksumKey", payosChecksumKey)
    .WithEnvironment("Google__Type", googleType)
    .WithEnvironment("Google__ProjectId", googleProjectId)
    .WithEnvironment("Google__PrivateKeyId", googlePrivateKeyId)
    .WithEnvironment("Google__PrivateKey", googlePrivateKey)
    .WithEnvironment("Google__ClientEmail", googleClientEmail)
    .WithEnvironment("Google__ClientId", googleClientId)
    .WithEnvironment("Google__AuthUri", googleAuthUri)
    .WithEnvironment("Google__TokenUri", googleTokenUri)
    .WithEnvironment("Google__AuthProviderX509CertUrl", googleAuthProviderCertUrl)
    .WithEnvironment("Google__ClientX509CertUrl", googleClientCertUrl)
    .WithEnvironment("Google__UniverseDomain", googleUniverseDomain)
    .WithReference(pentoDb)
    .WaitFor(pentoDb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(blobs)
    .WithHttpHealthCheck("/health/ready");
if (builder.ExecutionContext.IsRunMode)
{
    project.WithReference(seq);
}
var webhookUrl = ReferenceExpression.Create(
    $"{project.GetEndpoint("https").Property(EndpointProperty.Url)}/payments/webhook/handle");
var returnUrl = ReferenceExpression.Create($"pento://payments");
var cancelUrl = ReferenceExpression.Create($"pento://payments");
project.WithEnvironment("PayOS__WebhookUrl", webhookUrl)
         .WithEnvironment("PayOS__ReturnUrl", returnUrl)
         .WithEnvironment("PayOS__CancelUrl", cancelUrl);
builder.AddAzureContainerAppEnvironment("cae");
builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.Pento_AppHost>(
        [
            "DcpPublisher:RandomizePorts=false"
        ],
        configureBuilder: (appOptions, hostSettings) =>
        {
            appOptions.DisableDashboard = false;
        });
await builder.Build().RunAsync();
