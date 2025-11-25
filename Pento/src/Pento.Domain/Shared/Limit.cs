namespace Pento.Domain.Shared;

public sealed record Limit(int Quota, TimeUnit? ResetPer);
