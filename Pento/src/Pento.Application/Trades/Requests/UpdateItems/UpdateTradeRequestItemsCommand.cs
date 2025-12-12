using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.UpdateItems;

public sealed record UpdateTradeRequestItemsCommand(Guid RequestId, List<UpdateTradeItemDto> Items) : ICommand;
