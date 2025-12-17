using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeReportMedia : Entity
{
    private TradeReportMedia() { }

    public Guid TradeReportId { get; private set; }

    public TradeReportMediaType MediaType { get; private set; }

    public Uri MediaUri { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public TradeReportMedia(
        Guid id,
        Guid tradeReportId,
        TradeReportMediaType mediaType,
        Uri mediaUri,
        DateTime createdOn
    ) : base(id)
    {
        TradeReportId = tradeReportId;
        MediaType = mediaType;
        MediaUri = mediaUri;
        CreatedOn = createdOn;
    }

    public static TradeReportMedia Create(
        Guid tradeReportId,
        TradeReportMediaType mediaType,
        Uri mediaUri,
        DateTime createdOn
    )
    {
        return new TradeReportMedia(
            id: Guid.CreateVersion7(),
            tradeReportId: tradeReportId,
            mediaType: mediaType,
            mediaUri: mediaUri,
            createdOn: createdOn
        );
    }
    public enum TradeReportMediaType
    {
        Image,
        Video
    }
}
