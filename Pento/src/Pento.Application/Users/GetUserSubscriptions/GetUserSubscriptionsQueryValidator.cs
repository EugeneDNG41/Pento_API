using FluentValidation;

namespace Pento.Application.Users.GetUserSubscriptions;

internal sealed class GetUserSubscriptionsQueryValidator : AbstractValidator<GetUserSubscriptionsQuery>
{
    public GetUserSubscriptionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required.");
        RuleFor(x => x.FromDuration)
            .GreaterThanOrEqualTo(0).WithMessage("From Duration must be greater than or equal to 0.")
            .When(x => x.FromDuration.HasValue);
        RuleFor(x => x.ToDuration)
            .GreaterThanOrEqualTo(0).WithMessage("To Duration must be greater than or equal to 0.")
            .When(x => x.ToDuration.HasValue);
        RuleFor(x => x)
            .Must(x => !x.FromDuration.HasValue || !x.ToDuration.HasValue || x.FromDuration <= x.ToDuration)
            .WithMessage("From Duration must be less than or equal to To Duration.");
    }
}
