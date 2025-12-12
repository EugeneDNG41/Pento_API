using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Cancel;

public sealed record CancelTradeRequestCommand(Guid RequestId) : ICommand;
