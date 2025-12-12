using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Reject;

public sealed record RejectTradeRequestCommand(Guid RequestId) : ICommand;
