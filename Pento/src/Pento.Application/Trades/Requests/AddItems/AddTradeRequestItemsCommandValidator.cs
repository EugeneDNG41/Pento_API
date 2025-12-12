using FluentValidation;

namespace Pento.Application.Trades.Requests.AddItems;

internal sealed class AddTradeRequestItemsCommandValidator : AbstractValidator<AddTradeRequestItemsCommand>
{
    public AddTradeRequestItemsCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one trade item must be provided.");
        RuleForEach(x => x.Items).SetValidator(new AddTradeItemDtoValidator());
    }
}
