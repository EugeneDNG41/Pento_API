using FluentValidation;

namespace Pento.Application.Trades.Requests.UpdateItems;

internal sealed class UpdateTradeRequestItemsCommandValidator : AbstractValidator<UpdateTradeRequestItemsCommand>
{
    public UpdateTradeRequestItemsCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one trade item must be provided.");
        RuleForEach(x => x.Items).SetValidator(new UpdateTradeItemDtoValidator());
    }
}
