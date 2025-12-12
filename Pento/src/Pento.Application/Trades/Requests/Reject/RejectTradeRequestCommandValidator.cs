using FluentValidation;

namespace Pento.Application.Trades.Requests.Reject;

internal sealed class RejectTradeRequestCommandValidator : AbstractValidator<RejectTradeRequestCommand>
{
    public RejectTradeRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
    }
}
