using FluentValidation;

namespace Pento.Application.Subscriptions.GetById;

internal sealed class GetSubscriptionsQueryValidator : AbstractValidator<GetSubscriptionsQuery>
{
    public GetSubscriptionsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}
