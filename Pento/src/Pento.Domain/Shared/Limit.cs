namespace Pento.Domain.Shared;

public sealed record Limit(int Quota, Period? ResetPeriod);
