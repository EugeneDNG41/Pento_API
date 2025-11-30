using FluentValidation;

namespace Pento.Application.UserSubscriptions.GetUserSubscriptionById;

internal sealed class GetUserSubscriptionByIdQueryValidator : AbstractValidator<GetUserSubscriptionByIdQuery>
{
    public GetUserSubscriptionByIdQueryValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User Subscription Id is required.");
    }
}
