namespace Pento.Infrastructure.ThirdPartyServices.Identity;

internal sealed record CredentialRepresentation(string Type, string Value, bool Temporary);
