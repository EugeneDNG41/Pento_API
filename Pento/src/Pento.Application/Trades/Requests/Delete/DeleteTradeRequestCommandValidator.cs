using FluentValidation;

namespace Pento.Application.Trades.Requests.Delete;

internal sealed class DeleteTradeRequestCommandValidator : AbstractValidator<DeleteTradeRequestCommand>
{
    public DeleteTradeRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
    }
}
