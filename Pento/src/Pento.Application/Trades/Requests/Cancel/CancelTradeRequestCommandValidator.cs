using FluentValidation;

namespace Pento.Application.Trades.Requests.Cancel;

internal sealed class CancelTradeRequestCommandValidator : AbstractValidator<CancelTradeRequestCommand>
{
    public CancelTradeRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
    }
}
