using Newtonsoft.Json;

namespace Pento.Infrastructure.External.Google;

public sealed class GoogleOptions
{
    [JsonProperty("type")]
    public string Type { get; init; }

    [JsonProperty("project_id")]
    public string ProjectId { get; init; }

    [JsonProperty("private_key_id")]
    public string PrivateKeyId { get; init; }

    [JsonProperty("private_key")]
    public string PrivateKey { get; init; }

    [JsonProperty("client_email")]
    public string ClientEmail { get; init; }

    [JsonProperty("client_id")]
    public string ClientId { get; init; }

    [JsonProperty("auth_uri")]
    public string AuthUri { get; init; }

    [JsonProperty("token_uri")]
    public string TokenUri { get; init; }

    [JsonProperty("auth_provider_x509_cert_url")]
    public string AuthProviderX509CertUrl { get; init; }

    [JsonProperty("client_x509_cert_url")]
    public string ClientX509CertUrl { get; init; }

    [JsonProperty("universe_domain")]
    public string UniverseDomain { get; init; }
}
