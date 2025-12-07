namespace Pento.Application.Abstractions.Utility.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
    DateTimeOffset UtcNowOffset { get; }
}
