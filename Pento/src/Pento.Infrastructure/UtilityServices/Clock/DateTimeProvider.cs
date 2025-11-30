using Pento.Application.Abstractions.UtilityServices.Clock;

namespace Pento.Infrastructure.UtilityServices.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}
