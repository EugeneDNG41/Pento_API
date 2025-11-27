
using FluentValidation;

namespace Pento.Application.Payments.Create;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand> 
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(c => c.SubscriptionPlanId)
            .NotEmpty().WithMessage("Subscription Plan Code is required.");
    }
}
