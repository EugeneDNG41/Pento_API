namespace Pento.Infrastructure.External.Identity;

internal sealed record CredentialRepresentation(string Type, string Value, bool Temporary);
