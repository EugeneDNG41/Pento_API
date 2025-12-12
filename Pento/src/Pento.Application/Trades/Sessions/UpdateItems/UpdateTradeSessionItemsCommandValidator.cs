using FluentValidation;
using Pento.Application.Trades.Requests.Accept;

namespace Pento.Application.Trades.Sessions.UpdateItems;

internal sealed class UpdateTradeSessionItemsCommandValidator : AbstractValidator<UpdateTradeSessionItemsCommand>
{
    public UpdateTradeSessionItemsCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade session Id is required.");
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Trade tradeItem Ids must not be null.")
            .Must(ids => ids.Count > 0).WithMessage("At least one trade tradeItem ID must be provided.");
        RuleForEach(x => x.Items).SetValidator(new UpdateTradeItemDtoValidator());
    }
}
