using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Accept;

public sealed record AcceptTradeRequestCommand(Guid OfferId, Guid RequestId) : ICommand<Guid>;
public sealed record CancelTradeRequestCommand(Guid RequestId) : ICommand;
