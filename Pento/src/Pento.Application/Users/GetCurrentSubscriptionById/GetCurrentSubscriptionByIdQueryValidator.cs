using FluentValidation;

namespace Pento.Application.Users.GetCurrentSubscriptionById;

internal sealed class GetCurrentSubscriptionByIdQueryValidator : AbstractValidator<GetCurrentSubscriptionByIdQuery>
{
    public GetCurrentSubscriptionByIdQueryValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User Subscription Id is required.");
    }
}
