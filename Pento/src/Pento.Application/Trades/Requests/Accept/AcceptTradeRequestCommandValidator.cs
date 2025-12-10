using FluentValidation;

namespace Pento.Application.Trades.Requests.Accept;

internal sealed class AcceptTradeRequestCommandValidator : AbstractValidator<AcceptTradeRequestCommand>
{
    public AcceptTradeRequestCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty().WithMessage("Offer Id is required.");
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request Id is required.");
    }
}
