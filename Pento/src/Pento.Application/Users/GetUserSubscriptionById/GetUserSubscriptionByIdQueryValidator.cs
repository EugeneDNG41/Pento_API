using FluentValidation;

namespace Pento.Application.Users.GetUserSubscriptionById;

internal sealed class GetUserSubscriptionByIdQueryValidator : AbstractValidator<GetUserSubscriptionByIdQuery>
{
    public GetUserSubscriptionByIdQueryValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User Subscription Id is required.");
    }
}
