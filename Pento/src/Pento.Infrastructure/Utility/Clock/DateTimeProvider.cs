using Pento.Application.Abstractions.Utility.Clock;

namespace Pento.Infrastructure.Utility.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}
