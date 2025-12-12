using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Delete;

public sealed record DeleteTradeRequestCommand(Guid RequestId) : ICommand;
